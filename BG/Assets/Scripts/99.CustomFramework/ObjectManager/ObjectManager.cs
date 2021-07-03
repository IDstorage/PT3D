using System.Collections.Generic;
using System.Reflection;

namespace CustomFramework {

    public static class ObjectManager {
        // private ObjectManager() {}
        // private static ObjectManager instance = null;
        // public static ObjectManager Instance => instance ?? (instance = new ObjectManager());


        static Dictionary<EFunctionType, CustomList<CustomMethodBinder>> objectList = new Dictionary<EFunctionType, CustomList<CustomMethodBinder>>(new FunctionTypeExt());

#region For FunctionType
        public enum EFunctionType : int {
            START,
            ACTIVATE,
            DEACTIVATE,
            FIXEDUPDATE,
            FIXEDUPDATE_ISO,
            UPDATE,
            UPDATE_ISO,
            LATEUPDATE,
            LATEUPDATE_ISO
        }

        public struct FunctionTypeExt : IEqualityComparer<EFunctionType> {
            public bool Equals(EFunctionType a, EFunctionType b) {
                return a == b;
            }
            
            public int GetHashCode(EFunctionType f) {
                return (int)f;
            }
        }

        public class CustomMethodBinder {
            public CustomBehaviour target;
            public MethodInfo method;
            public bool Invoke(params object[] pl) {
                if (target == null || method == null) return true;
                method.Invoke(target, pl);
                return false;
            }
        }
#endregion

        public static void Register(CustomBehaviour obj, MethodInfo m, EFunctionType type) {
            if (obj == null) return;
            if (!objectList.ContainsKey(type)) objectList.Add(type, CustomList<CustomMethodBinder>.Create(new CustomMethodBinder() { target = obj, method = m }));
            else objectList[type].Add(new CustomMethodBinder() { target = obj, method = m });
            if (type == EFunctionType.START) m.Invoke(obj, null);
        }
        

        static bool IsNull(EFunctionType type, CustomList<CustomMethodBinder> target) {
            if (!objectList.ContainsKey(type)) return true;
            return false;
        }


        public static void CheckActivation() {
            if (!objectList.ContainsKey(EFunctionType.ACTIVATE)) return;

            CustomList<CustomMethodBinder> search = objectList[EFunctionType.ACTIVATE];
            while (search != null) {
                //if (IsNull(EFunctionType.ACTIVATE, search)) continue;

                var target = search.data;
                bool activeInHierarchy = target.target.gameObject.activeInHierarchy;

                if (target.target.cachedActiveFlag == activeInHierarchy) {
                    search = search.next;
                    continue;
                }

                if (activeInHierarchy) target.Invoke();
                else target.Invoke();

                target.target.cachedActiveFlag = activeInHierarchy;

                search = search.next;
            }
        }


        public static void FixedUpdate() {
            CustomPhysics.Update();
            UpdateLoop(EFunctionType.FIXEDUPDATE, false);
            UpdateLoop(EFunctionType.FIXEDUPDATE_ISO, true);
        }

        public static void Update() {
            UpdateLoop(EFunctionType.UPDATE, false);
            UpdateLoop(EFunctionType.UPDATE_ISO, true);
        }

        public static void LateUpdate() {
            UpdateLoop(EFunctionType.LATEUPDATE, false);
            UpdateLoop(EFunctionType.LATEUPDATE_ISO, true);
        }

        static void UpdateLoop(EFunctionType type, bool isIsolated) {
            CustomList<CustomMethodBinder> search = objectList.ContainsKey(type) ? objectList[type] : null;
            while (search != null) {
                //if (IsNull(type, search)) continue;

                var target = search.data;

                if (target.target.gameObject.activeInHierarchy == false && !isIsolated) {
                    search = search.next;
                    continue;
                }

                target.Invoke();

                search = search.next;
            }
        }
    }

}