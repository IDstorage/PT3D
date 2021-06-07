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
                        self.OnCollidedStay(target);
                    }
                    else {
                        self.collisionList.Add(target);
                        self.OnCollidedEnter(target);
                    }
                }
                else if (self.collisionList.Contains(target)) {
                    self.collisionList.Remove(target);
                    self.OnCollidedEnd(target);
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

            float near = 0F;
            while (search != null) {
                if (search.collider == null) {
                    search = search.next;
                    continue;
                }
                if (search.collider.IgnoreRaycast) {
                    search = search.next;
                    continue;
                }

                if (RayWithOBB(search.collider, out Vector3 hitPoint)) {
                    //Debug.LogError(search.collider.name);
                    if (result == null) result = new RaycastHit() {
                        hitPoint = hitPoint,
                        target = search.collider
                    };
                    else if ((result.hitPoint - result.target.Center).magnitude > (hitPoint - search.collider.Center).magnitude) {
                        result.hitPoint = hitPoint;
                        result.target = search.collider;
                    }
                }
                search = search.next;
            }

            return result;

            bool RayWithOBB(CustomCollider c, out Vector3 hitPoint) {
                string log = "";
                hitPoint = Vector3.zero;

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
                for (int i = 0; i < points.Length; ++i) {
                    Vector2 v = (cvOrigin + (Vector2.Dot(points[i] - cvOrigin, cvDir) / cvDir.sqrMagnitude) * cvDir) - points[i];
                    if (i == 0) {
                        std = v;
                        continue;
                    }
                    // Overlap
                    if (Vector3.Dot(std, v) <= 0F) {
                        checkCount++;
                        log += "x";
                        break;
                    }
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

                for (int i = 0; i < points.Length; ++i) {
                    Vector2 v = (cvOrigin + (Vector2.Dot(points[i] - cvOrigin, cvDir) / cvDir.sqrMagnitude) * cvDir) - points[i];
                    if (i == 0) {
                        std = v;
                        continue;
                    }
                    // Overlap
                    if (Vector2.Dot(std, v) <= 0F) {
                        checkCount++;
                        log += "y";
                        break;
                    }
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

                for (int i = 0; i < points.Length; ++i) {
                    Vector2 v = (cvOrigin + (Vector2.Dot(points[i] - cvOrigin, cvDir) / cvDir.sqrMagnitude) * cvDir) - points[i];
                    if (i == 0) {
                        std = v;
                        continue;
                    }
                    // Overlap
                    if (Vector3.Dot(std, v) <= 0F) {
                        checkCount++;
                        log += "z";
                        break;
                    }
                }
                #endregion

                Debug.LogError($"{log} / {checkCount}");
                return checkCount == 3;
            }
        }
    }

}