using System.Reflection;
using CustomFramework;
using UnityEngine;

public class CustomBehaviour : MonoBehaviour {

    [System.NonSerialized] public bool cachedActiveFlag = false;

    GameObject _gameObject = null;
    new public GameObject gameObject {
        get {
            if (_gameObject == null) return base.gameObject;
            return _gameObject;
        }
    }

    Transform _transform = null;
    new public Transform transform {
        get {
            if (_transform == null) return base.transform;
            return _transform;
        }
    }


    void Awake() {
        Initialize();
    }

    void Initialize() {
        MethodInfo GetMethod(string methodName) {
            return GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        bool IsValidMagicFunction(MethodInfo method) {
            return method != null && method.GetParameters().Length == 0 && method.ReturnType == typeof(void);
        }
        bool IsValidCollisionFunction(MethodInfo method) {
            return method != null && method.GetParameters().Length == 1 && method.ReturnType == typeof(void) && method.GetParameters()[0].ParameterType == typeof(CustomCollider);
        }

        _gameObject = base.gameObject;
        _transform = base.transform;
        
        var self = this;

        if (IsValidMagicFunction(GetMethod("OnStart"))) {
            ObjectManager.Register(this, GetMethod("OnStart"), ObjectManager.EFunctionType.START);
        }
        if (IsValidMagicFunction(GetMethod("OnActivate"))) {
            ObjectManager.Register(this, GetMethod("OnActivate"), ObjectManager.EFunctionType.ACTIVATE);
        }
        if (IsValidMagicFunction(GetMethod("OnDeactivate"))) {
            ObjectManager.Register(this, GetMethod("OnDeactivate"), ObjectManager.EFunctionType.DEACTIVATE);
        }
        if (IsValidMagicFunction(GetMethod("OnFixedUpdate"))) {
            ObjectManager.Register(this, GetMethod("OnFixedUpdate"), ObjectManager.EFunctionType.FIXEDUPDATE);
        }
        if (IsValidMagicFunction(GetMethod("OnISOFixedUpdate"))) { 
            ObjectManager.Register(this, GetMethod("OnISOFixedUpdate"), ObjectManager.EFunctionType.FIXEDUPDATE_ISO);
        }
        if (IsValidMagicFunction(GetMethod("OnUpdate"))) {
            ObjectManager.Register(this, GetMethod("OnUpdate"), ObjectManager.EFunctionType.UPDATE);
        }
        if (IsValidMagicFunction(GetMethod("OnISOUpdate"))) {
            ObjectManager.Register(this, GetMethod("OnISOUpdate"), ObjectManager.EFunctionType.UPDATE_ISO);
        }
        if (IsValidMagicFunction(GetMethod("OnLateUpdate"))) {
            ObjectManager.Register(this, GetMethod("OnLateUpdate"), ObjectManager.EFunctionType.LATEUPDATE);
        }
        if (IsValidMagicFunction(GetMethod("OnISOLateUpdate"))) {
            ObjectManager.Register(this, GetMethod("OnISOLateUpdate"), ObjectManager.EFunctionType.LATEUPDATE_ISO);
        }


        if (IsValidCollisionFunction(GetMethod("OnCollidedEnter"))) {
            var component = GetComponent<CustomCollider>();
            if (component != null) component._OnCollidedEnter = new ObjectManager.CustomMethodBinder { target = this, method = GetMethod("OnCollidedEnter") };
        }
        if (IsValidCollisionFunction(GetMethod("OnCollidedStay"))) {
            var component = GetComponent<CustomCollider>();
            if (component != null) component._OnCollidedStay = new ObjectManager.CustomMethodBinder { target = this, method = GetMethod("OnCollidedStay") };       
        }
        if (IsValidCollisionFunction(GetMethod("OnCollidedEnd"))) {
            var component = GetComponent<CustomCollider>();
            if (component != null) component._OnCollidedEnd = new ObjectManager.CustomMethodBinder { target = this, method = GetMethod("OnCollidedEnd") };
        }
    }
}
