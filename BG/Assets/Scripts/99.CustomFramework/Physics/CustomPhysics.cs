using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomFramework {

    // 공간분할 하지 않은 상태
    public static class CustomPhysics {

        public static readonly int MAXIMUM_BOUND_WIDTH = 3;

        public class PosIndex {
            public int x = 0;
            public int y = 0;
            public int z = 0;
            public int radius = 0;

            public bool Equals(PosIndex a) {
                return ((a.x - this.x) * (a.x - this.x) + (a.y - this.y) * (a.y - this.y) + (a.z - this.z) * (a.z - this.z)) < ((a.radius + this.radius) * (a.radius + this.radius));
            }
        }


        static Unit colliderList = null;
        static Unit dynamicColliderList = null;


        public class Unit {
            public CustomCollider collider;
            public Unit next = null;
        }


        public static void Add(CustomCollider c) {
            Add(ref colliderList, c);
            if (c.IsDynamic) Add(ref dynamicColliderList, c);
        }
        static void Add(ref Unit target, CustomCollider c) {
            Unit unit = new Unit() {
                collider = c,
                next = target
            };
            target = unit;
        }

        public static void Remove(CustomCollider c) {
            Remove(ref colliderList, c);
            if (c.IsDynamic) Remove(ref dynamicColliderList, c);
        }
        static void Remove(ref Unit target, CustomCollider c) {
            if (target == null || c == null) return;

            Unit search = target;
            if (ReferenceEquals(search, c)) {
                target = search.next;
                return;
            }

            while (search.next != null) {
                if (ReferenceEquals(search.next.collider, c)) {
                    search.next = search.next.next;
                    return;
                }
                search = search.next;
            }
        }

        public static void UpdateState(CustomCollider c, bool isDynamic) {
            if (isDynamic) {
                Add(ref dynamicColliderList, c);
            }
            else {
                Remove(ref dynamicColliderList, c);
            }
        }

        static int frameCount = 0;
        public static void Update() {
            frameCount++;
            if (frameCount > 5) return;
            frameCount = 0;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            int cnt = 0;

            Unit search = dynamicColliderList;
            while (search != null) {
                if (search.collider == null) {
                    search = search.next;
                    continue;
                }
                search.collider.UpdateIndex();
                CheckCollision(search);
                search = search.next;
            }

            void CheckCollision(Unit target) {
                if (target.collider == null) {
                    Remove(target.collider);
                    target = target.next;
                    return;
                }

                if (!target.collider.gameObject.activeInHierarchy) {
                    target = target.next;
                    return;
                }

                Unit other = colliderList;
                while (other != null) {
                    if (other.collider == null) {
                        Remove(other.collider);
                        other = other.next;
                        continue;
                    }

                    if (ReferenceEquals(target.collider, other.collider) || !other.collider.gameObject.activeInHierarchy) {
                        other = other.next;
                        continue;
                    }
                    cnt++;

                    other.collider.UpdateIndex();
                    if (!target.collider.Index.Equals(other.collider.Index)) {
                        other = other.next;
                        continue;
                    }

                    bool result = target.collider.Collide(other.collider);
                    HandleAfterCollision(target.collider, other.collider, result);
                    other = other.next;
                }
            }

            void HandleAfterCollision(CustomCollider self, CustomCollider target, bool result) {
                if (result) {
                    if (self.collisionList.Contains(target)) {
                        if (self.OnCollidedStay != null) self.OnCollidedStay(target);
                    }
                    else {
                        self.collisionList.Add(target);
                        if (self.OnCollidedEnter != null) self.OnCollidedEnter(target);
                    }
                }
                else if (self.collisionList.Contains(target)) {
                    self.collisionList.Remove(target);
                    if (self.OnCollidedEnd != null) self.OnCollidedEnd(target);
                }
            }

            sw.Stop();
            //Debug.LogError($"{sw.ElapsedMilliseconds} / {cnt}");
        }


        public class RaycastHit {
            public Vector3 hitPoint;
            public CustomCollider target;
        }
        
        public static RaycastHit Raycast(Vector3 origin, Vector3 dir, float distance) {
            dir = dir.normalized;
            Vector3 ray = origin + dir;

            Unit search = colliderList;
            RaycastHit result = null;
            Vector3 hitPoint;

            while (search != null) {
                if (search.collider == null) {
                    search = search.next;
                    continue;
                }
                if (search.collider.IgnoreRaycast) {
                    search = search.next;
                    continue;
                }

                if (RayWithOBB(search.collider)) {
                    if (result == null) result = new RaycastHit() {
                        //hitPoint = hitPoint,
                        target = search.collider
                    };
                }
                search = search.next;
            }

            return result;

            bool RayWithOBB(CustomCollider c) {
                var center = c.Center;

                var invRot = Quaternion.Inverse(c.transform.rotation);

                var rotOrigin = invRot * (origin - center) + center;
                var rotDest = invRot * (origin + dir * distance - center) + center;
                var rotDir = (rotDest - rotOrigin).normalized;

                var min = center - c.size * 0.5f;
                var max = center + c.size * 0.5f;

#if UNITY_EDITOR && DEBUG_RAYCAST
                Debug.DrawLine(rotOrigin, rotDest, Color.blue);
                Debug.DrawLine(rotOrigin, rotOrigin + Vector3.up, Color.yellow);

                Debug.DrawLine(min, min + Vector3.right * c.size.x, Color.magenta);
                Debug.DrawLine(min, min + Vector3.up * c.size.y, Color.magenta);
                Debug.DrawLine(min, min + Vector3.forward * c.size.z, Color.magenta);
                Debug.DrawLine(max, max - Vector3.right * c.size.x, Color.magenta);
                Debug.DrawLine(max, max - Vector3.up * c.size.y, Color.magenta);
                Debug.DrawLine(max, max - Vector3.forward * c.size.z, Color.magenta);
#endif

                float tMin = (min.x - rotOrigin.x) / rotDir.x;
                float tMax = (max.x - rotOrigin.x) / rotDir.x;

                if (tMin > tMax) { float temp = tMin; tMin = tMax; tMax = temp; }

                float tyMin = (min.y - rotOrigin.y) / rotDir.y;
                float tyMax = (max.y - rotOrigin.y) / rotDir.y;

                if (tyMin > tyMax) { float temp = tyMin; tyMin = tyMax; tyMax = temp; }

                if ((tMin > tyMax) || (tyMin > tMax)) return false;

                if (tyMin > tMin) tMin = tyMin;
                if (tyMax < tMax) tMax = tyMax;

                float tzMin = (min.z - rotOrigin.z) / rotDir.z;
                float tzMax = (max.z - rotOrigin.z) / rotDir.z;

                if (tzMin > tzMax) { float temp = tzMin; tzMin = tzMax; tzMax = temp; }

                if ((tMin > tzMax) || (tzMin > tMax)) return false;

                if (tMin > distance || tyMin > distance || tzMin > distance) return false;
                if (tMax < 0f || tyMax < 0f || tzMax < 0f) return false;

                return true;
            }

            #region NOT USING
            /*bool RayWithOBB(CustomCollider c) {
                string log = "";

                int checkCount = 0;

                var center = c.Center;

                #region Ignore Z
                Vector2 cvCenter = new Vector2(center.x, center.y);
                Vector2 cvOrigin = new Vector2(origin.x, origin.y), cvDir = new Vector2(dir.x, dir.y);
                Vector2 right = new Vector2(c.axis[0].x, c.axis[0].y),
                        up = new Vector2(c.axis[1].x, c.axis[1].y);
                Vector2[] points = new Vector2[4] {
                    cvCenter - (right * c.size.x + up * c.size.y) * 0.5f,
                    cvCenter - (-right * c.size.x + up * c.size.y) * 0.5f,
                    cvCenter + (-right * c.size.x + up * c.size.y) * 0.5f,
                    cvCenter + (right * c.size.x + up * c.size.y) * 0.5f
                };

                Vector2 std = Vector2.zero;
                int validCount = 0; bool diff = false;

                //if (Mathf.Min(points[0].x, points[1].x) <= cvCenter.x && cvCenter.x <= Mathf.Max(points[0].x, points[1].x)
                //    && Mathf.Min(points[1].y, points[2].y) <= cvCenter.y && cvCenter.y <= Mathf.Max(points[1].y, points[2].y)) {
                //    diff = true; validCount++;
                //}

                for (int i = 0; i < points.Length; ++i) {
                    Vector2 v1 = cvOrigin + (Vector2.Dot(points[i] - cvOrigin, cvDir) / cvDir.sqrMagnitude) * cvDir;
                    Vector2 v2 = v1 - points[i];
                    if (i == 0) std = v2;
                    //Debug.LogError($"0 <= {(v - cvOrigin).magnitude} <= {distance}");
                    //Debug.DrawRay(new Vector3(v.x, v.y, origin.z), Vector3.up, Color.blue);
                    //var d = v2 - cvOrigin;
                    if ((v1 - cvOrigin).magnitude <= distance) validCount++;
                    // Overlap
                    if (i != 0 && Vector2.Dot(std, v2) <= 0F && !diff) {
                        diff = diff || Vector2.Dot(v1, cvDir) >= 0F;
                    }
                }
                //Debug.LogError($"{checkCount} / {diff} / {validCount}");
                if (diff && validCount >= 1) {
                    checkCount++;
                    diff = false;
                    log += "x";
                }
                #endregion

                #region Ignore Y
                cvCenter.x = center.x; cvCenter.y = center.z;
                cvOrigin.x = origin.x; cvOrigin.y = origin.z;
                cvDir.x = dir.x; cvDir.y = dir.z;
                right.x = c.axis[0].x; right.y = c.axis[0].z;
                Vector2 forward = new Vector2(c.axis[2].x, c.axis[2].z);
                points[0] = cvCenter - (right * c.size.x + forward * c.size.z) * 0.5f;
                points[1] = cvCenter - (-right * c.size.x + forward * c.size.z) * 0.5f;
                points[2] = cvCenter + (-right * c.size.x + forward * c.size.z) * 0.5f;
                points[3] = cvCenter + (right * c.size.x + forward * c.size.z) * 0.5f;

                validCount = 0;

                //if (Mathf.Min(points[0].x, points[1].x) <= cvCenter.x && cvCenter.x <= Mathf.Max(points[0].x, points[1].x)
                //    && Mathf.Min(points[1].y, points[2].y) <= cvCenter.y && cvCenter.y <= Mathf.Max(points[1].y, points[2].y)) {
                //    diff = true; validCount++;
                //}

                for (int i = 0; i < points.Length; ++i) {
                    Vector2 v1 = cvOrigin + (Vector2.Dot(points[i] - cvOrigin, cvDir) / cvDir.sqrMagnitude) * cvDir;
                    Vector2 v2 = v1 - points[i];
                    if (i == 0) std = v2;
                    //Debug.LogError($"0 <= {(v - cvOrigin).magnitude} <= {distance}");
                    //Debug.DrawRay(new Vector3(v.x, origin.y, v.y), Vector3.up, Color.blue);
                    //var d = v1 - cvOrigin;
                    if ((v1 - cvOrigin).magnitude <= distance) validCount++;
                    // Overlap
                    if (i != 0 && Vector2.Dot(std, v2) <= 0F && !diff) {
                        diff = diff || Vector2.Dot(v1, cvDir) >= 0F;
                    }
                }
                //Debug.LogError($"{checkCount} / {diff} / {validCount}");
                if (diff && validCount >= 1) {
                    checkCount++;
                    log += "y";
                    diff = false;
                }
                #endregion

                #region Ignore X
                cvCenter.x = center.y; cvCenter.y = center.z;
                cvOrigin.x = origin.y; cvOrigin.y = origin.z;
                cvDir.x = dir.y; cvDir.y = dir.z;
                up.x = c.axis[1].y; up.y = c.axis[1].z;
                forward.x = c.axis[2].y; forward.y = c.axis[2].z;
                points[0] = cvCenter - (up * c.size.y + forward * c.size.z) * 0.5f;
                points[1] = cvCenter - (-up * c.size.y + forward * c.size.z) * 0.5f;
                points[2] = cvCenter + (-up * c.size.y + forward * c.size.z) * 0.5f;
                points[3] = cvCenter + (up * c.size.y + forward * c.size.z) * 0.5f;

                validCount = 0;

                //if (Mathf.Min(points[0].x, points[1].x) <= cvCenter.x && cvCenter.x <= Mathf.Max(points[0].x, points[1].x)
                //    && Mathf.Min(points[1].y, points[2].y) <= cvCenter.y && cvCenter.y <= Mathf.Max(points[1].y, points[2].y)) {
                //    diff = true; validCount++;
                //}

                for (int i = 0; i < points.Length; ++i) {
                    Vector2 v1 = cvOrigin + (Vector2.Dot(points[i] - cvOrigin, cvDir) / cvDir.sqrMagnitude) * cvDir;
                    Vector2 v2 = v1 - points[i];
                    if (i == 0) std = v2;
                    //Debug.LogError($"0 <= {(v - cvOrigin).magnitude} <= {distance}");
                    //Debug.DrawRay(new Vector3(origin.x, v.x, v.y), Vector3.up, Color.blue);
                    //var d = v - cvOrigin;
                    if ((v1 - cvOrigin).magnitude <= distance) validCount++;
                    // Overlap
                    if (i != 0 && Vector2.Dot(std, v2) <= 0F && !diff) {
                        diff = diff || Vector2.Dot(v1 - cvOrigin, cvDir) >= 0F;
                    }
                }
                //Debug.LogError($"{checkCount} / {diff} / {validCount}");
                if (diff && validCount >= 1) {
                    checkCount++;
                    log += "z";
                    diff = false;
                }
                #endregion

                Debug.LogError($"{c.name} / {log} / {checkCount == 3}");
                return checkCount == 3;
            }
            bool RayWithOBB2(CustomCollider c) {
                int CCW(Vector2 ray1, Vector2 ray2, Vector3 pt) {
                    float ans = (ray2.x - ray1.x) * (pt.y - ray1.y) - (ray2.y - ray1.y) * (pt.x - ray1.x);
                    if (ans < 0F) return 1;
                    else if (ans > 0F) return -1;
                    else return 0;
                }

                string log = "";

                int checkCount = 0;

                var center = c.Center;
                Vector3 dest = origin + dir * distance;

                #region Ignore Z
                Vector2 cvCenter = new Vector2(center.x, center.y);
                Vector2 cvOrigin = new Vector2(origin.x, origin.y), cvDir = new Vector2(dir.x, dir.y);
                Vector2 right = new Vector2(c.axis[0].x, c.axis[0].y),
                        up = new Vector2(c.axis[1].x, c.axis[1].y);
                Vector2 cvDest = new Vector2(dest.x, dest.y);
                Vector2[] points = new Vector2[4] {
                    cvCenter - (right * c.size.x + up * c.size.y) * 0.5f,
                    cvCenter - (-right * c.size.x + up * c.size.y) * 0.5f,
                    cvCenter + (-right * c.size.x + up * c.size.y) * 0.5f,
                    cvCenter + (right * c.size.x + up * c.size.y) * 0.5f
                };

                Vector2 std = Vector2.zero;
                int validCount = 0; bool diff = false;

                bool isCollided =
                    ((CCW(cvOrigin, cvDest, points[0]) * CCW(cvOrigin, cvDest, points[1]) <= 0F) && (CCW(points[0], points[1], cvOrigin) * CCW(points[0], points[1], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[0]) * CCW(cvOrigin, cvDest, points[2]) <= 0F) && (CCW(points[0], points[2], cvOrigin) * CCW(points[0], points[2], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[1]) * CCW(cvOrigin, cvDest, points[3]) <= 0F) && (CCW(points[1], points[3], cvOrigin) * CCW(points[1], points[3], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[2]) * CCW(cvOrigin, cvDest, points[3]) <= 0F) && (CCW(points[2], points[3], cvOrigin) * CCW(points[2], points[3], cvDest) <= 0F));
                isCollided = isCollided
                    || (Mathf.Min(points[0].x, points[1].x) <= cvOrigin.x && cvOrigin.x <= Mathf.Max(points[0].x, points[1].x));
                if (isCollided) checkCount++;
                #endregion

                #region Ignore Y
                cvCenter.x = center.x; cvCenter.y = center.z;
                cvOrigin.x = origin.x; cvOrigin.y = origin.z;
                cvDir.x = dir.x; cvDir.y = dir.z;
                right.x = c.axis[0].x; right.y = c.axis[0].z;
                Vector2 forward = new Vector2(c.axis[2].x, c.axis[2].z);
                cvDest.x = dest.x; cvDest.y = dest.z;
                points[0] = cvCenter - (right * c.size.x + forward * c.size.z) * 0.5f;
                points[1] = cvCenter - (-right * c.size.x + forward * c.size.z) * 0.5f;
                points[2] = cvCenter + (-right * c.size.x + forward * c.size.z) * 0.5f;
                points[3] = cvCenter + (right * c.size.x + forward * c.size.z) * 0.5f;

                validCount = 0;

                isCollided =
                    ((CCW(cvOrigin, cvDest, points[0]) * CCW(cvOrigin, cvDest, points[1]) <= 0F) && (CCW(points[0], points[1], cvOrigin) * CCW(points[0], points[1], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[0]) * CCW(cvOrigin, cvDest, points[2]) <= 0F) && (CCW(points[0], points[2], cvOrigin) * CCW(points[0], points[2], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[1]) * CCW(cvOrigin, cvDest, points[3]) <= 0F) && (CCW(points[1], points[3], cvOrigin) * CCW(points[1], points[3], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[2]) * CCW(cvOrigin, cvDest, points[3]) <= 0F) && (CCW(points[2], points[3], cvOrigin) * CCW(points[2], points[3], cvDest) <= 0F));
                if (isCollided) checkCount++;
                #endregion

                #region Ignore X
                cvCenter.x = center.y; cvCenter.y = center.z;
                cvOrigin.x = origin.y; cvOrigin.y = origin.z;
                cvDir.x = dir.y; cvDir.y = dir.z;
                up.x = c.axis[1].y; up.y = c.axis[1].z;
                forward.x = c.axis[2].y; forward.y = c.axis[2].z;
                cvDest.x = dest.y; cvDest.y = dest.z;
                points[0] = cvCenter - (up * c.size.y + forward * c.size.z) * 0.5f;
                points[1] = cvCenter - (-up * c.size.y + forward * c.size.z) * 0.5f;
                points[2] = cvCenter + (-up * c.size.y + forward * c.size.z) * 0.5f;
                points[3] = cvCenter + (up * c.size.y + forward * c.size.z) * 0.5f;

                validCount = 0;

                isCollided =
                    ((CCW(cvOrigin, cvDest, points[0]) * CCW(cvOrigin, cvDest, points[1]) <= 0F) && (CCW(points[0], points[1], cvOrigin) * CCW(points[0], points[1], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[0]) * CCW(cvOrigin, cvDest, points[2]) <= 0F) && (CCW(points[0], points[2], cvOrigin) * CCW(points[0], points[2], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[1]) * CCW(cvOrigin, cvDest, points[3]) <= 0F) && (CCW(points[1], points[3], cvOrigin) * CCW(points[1], points[3], cvDest) <= 0F))
                    || ((CCW(cvOrigin, cvDest, points[2]) * CCW(cvOrigin, cvDest, points[3]) <= 0F) && (CCW(points[2], points[3], cvOrigin) * CCW(points[2], points[3], cvDest) <= 0F));
                if (isCollided) checkCount++;
                #endregion

                //Debug.LogError($"{log} / {checkCount == 3}");
                return checkCount == 3;
            }*/
            #endregion
        }
    }

}