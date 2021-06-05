using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomFramework {

    // 공간분할 하지 않은 상태
    public static class CustomPhysics {

        public class PosIndex : IEqualityComparer<PosIndex> {
            public int x, y, z;

            bool IEqualityComparer<PosIndex>.Equals(PosIndex a, PosIndex b) {
                return a.x == b.x && a.y == b.y && a.z == b.z;
            }

            int IEqualityComparer<PosIndex>.GetHashCode(PosIndex obj) {
                return obj.x + obj.y + obj.z;
            }
        }

        static Unit colliderList = null;
        static Unit dynamicColliderList = null;

        public class Unit {
            public CustomCollider collider;
            public Unit next = null;
        }


        public static void Add(CustomCollider c) {
            Unit newUnit = new Unit() {
                collider = c,
                next = colliderList
            };
            colliderList = newUnit;

            if (c.IsDynamic) AddDynamic(c);
        }
        static void AddDynamic(CustomCollider c) {
            Unit dUnit = new Unit() {
                collider = c,
                next = dynamicColliderList
            };
            dynamicColliderList = dUnit;
        }

        public static void Remove(CustomCollider c) {
            Unit search = colliderList;
            if (ReferenceEquals(search, c)) {
                colliderList = search.next;
                return;
            }

            while (search.next != null) {
                if (ReferenceEquals(search.next.collider, c)) {
                    search.next = search.next.next;
                    return;
                }
                search = search.next;
            }

            if (c.IsDynamic) RemoveDynamic(c);
        }
        static void RemoveDynamic(CustomCollider c) {
            Unit search = dynamicColliderList;
            if (ReferenceEquals(search, c)) {
                colliderList = search.next;
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
                AddDynamic(c);
            }
            else {
                RemoveDynamic(c);
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
                    Remove(search.collider);
                    search = search.next;
                    continue;
                }

                if (!search.collider.gameObject.activeInHierarchy) {
                    search = search.next;
                    continue;
                }

                Unit other = colliderList;
                while (other != null) {
                    if (other.collider == null) {
                        Remove(other.collider);
                        other = other.next;
                        continue;
                    }

                    if (ReferenceEquals(search.collider, other.collider) || !other.collider.gameObject.activeInHierarchy) {
                        other = other.next;
                        continue;
                    }
                    cnt++;
                    bool result = search.collider.Collide(other.collider);
                    HandleCollision(search.collider, other.collider, result);
                    other = other.next;
                }
                search = search.next;
            }

            void HandleCollision(CustomCollider self, CustomCollider target, bool result) {
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
            Debug.LogError($"{sw.ElapsedMilliseconds} / {cnt}");
        }
    }

}