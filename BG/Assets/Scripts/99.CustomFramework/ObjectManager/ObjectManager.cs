using System.Collections.Generic;

namespace CustomFramework {

    public static class ObjectManager {
        // private ObjectManager() {}
        // private static ObjectManager instance = null;
        // public static ObjectManager Instance => instance ?? (instance = new ObjectManager());


        static Dictionary<EFunctionType, CustomList<CustomBehaviour>> objectList = new Dictionary<EFunctionType, CustomList<CustomBehaviour>>(new FunctionTypeExt());

#region For FunctionType
        public enum EFunctionType : int {
            START,
            ACTIVATE,
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

#endregion

        public static void Register(CustomBehaviour obj, EFunctionType type) {
            if (obj == null) return;
            if (!objectList.ContainsKey(type)) objectList.Add(type, CustomList<CustomBehaviour>.Create(obj));
            else objectList[type].Add(obj);
            if (type == EFunctionType.START) obj.OnStart();
        }


        static bool IsNull(EFunctionType type, CustomList<CustomBehaviour> target) {
            if (!objectList.ContainsKey(type)) return true;
            
            return false;
        }


        public static void CheckActivation() {
            if (!objectList.ContainsKey(EFunctionType.ACTIVATE)) return;

            CustomList<CustomBehaviour> search = objectList[EFunctionType.ACTIVATE];
            while (search != null) {
                if (IsNull(EFunctionType.ACTIVATE, search)) continue;

                var target = search.data;
                bool activeInHierarchy = target.gameObject.activeInHierarchy;

                if (target.cachedActiveFlag == activeInHierarchy) continue;

                if (activeInHierarchy) target.OnActivate();
                else target.OnDeactivate();

                target.cachedActiveFlag = activeInHierarchy;

                search = search.next;
            }
        }


        public static void FixedUpdate() {
            UpdateLoop(EFunctionType.FIXEDUPDATE);
        }

        public static void Update() {
            UpdateLoop(EFunctionType.UPDATE);
        }

        public static void LateUpdate() {
            UpdateLoop(EFunctionType.LATEUPDATE);
        }

        static void UpdateLoop(EFunctionType type) {
            CustomList<CustomBehaviour> search = objectList.ContainsKey(type) ? objectList[type] : null;
            while (search != null) {
                if (IsNull(type, search)) continue;

                var target = search.data;

                if (target.gameObject.activeInHierarchy == false) continue;

                if (type == EFunctionType.FIXEDUPDATE) target.OnFixedUpdate();
                else if (type == EFunctionType.UPDATE) target.OnUpdate();
                else if (type == EFunctionType.LATEUPDATE) target.OnLateUpdate();

                search = search.next;
            }

            var isoType = type + 1;
            search = objectList.ContainsKey(isoType) ? objectList[isoType] : null;
            while (search != null)
            {
                if (IsNull(isoType, search)) continue;

                var target = search.data;

                if (target.gameObject.activeInHierarchy == false) continue;

                if (type == EFunctionType.FIXEDUPDATE) target.OnISOFixedUpdate();
                else if (type == EFunctionType.UPDATE) target.OnISOUpdate();
                else if (type == EFunctionType.LATEUPDATE) target.OnISOLateUpdate();

                search = search.next;
            }
        }
    }

}