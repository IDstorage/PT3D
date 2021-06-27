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
        bool IsOverriden(MethodInfo method) {
            return method.GetBaseDefinition().DeclaringType != method.DeclaringType;
        }

        _gameObject = base.gameObject;
        _transform = base.transform;
        
        var self = this;

        if (IsOverriden(this.GetType().GetMethod("OnStart"))) {
            ObjectManager.Register(this, ObjectManager.EFunctionType.START);
        }
        if (IsOverriden(this.GetType().GetMethod("OnActivate")) || IsOverriden(this.GetType().GetMethod("OnDeactivate"))) {
            ObjectManager.Register(this, ObjectManager.EFunctionType.ACTIVATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnFixedUpdate"))) {
            ObjectManager.Register(this, ObjectManager.EFunctionType.FIXEDUPDATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnISOFixedUpdate"))) { 
            ObjectManager.Register(this, ObjectManager.EFunctionType.FIXEDUPDATE_ISO);
        }
        if (IsOverriden(this.GetType().GetMethod("OnUpdate"))) {
            ObjectManager.Register(this, ObjectManager.EFunctionType.UPDATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnISOUpdate"))) {
            ObjectManager.Register(this, ObjectManager.EFunctionType.UPDATE_ISO);
        }
        if (IsOverriden(this.GetType().GetMethod("OnLateUpdate"))) {
            ObjectManager.Register(this, ObjectManager.EFunctionType.LATEUPDATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnISOLateUpdate"))) {
            ObjectManager.Register(this, ObjectManager.EFunctionType.LATEUPDATE_ISO);
        }


        if (IsOverriden(this.GetType().GetMethod("OnCollidedEnter"))) {
            var component = GetComponent<CustomCollider>();
            if (component != null) component._OnCollidedEnter += OnCollidedEnter;
        }
        if (IsOverriden(this.GetType().GetMethod("OnCollidedStay"))) {
            var component = GetComponent<CustomCollider>();
            if (component != null) component._OnCollidedStay += OnCollidedStay;
        }
        if (IsOverriden(this.GetType().GetMethod("OnCollidedEnd"))) {
            var component = GetComponent<CustomCollider>();
            if (component != null) component._OnCollidedEnd += OnCollidedEnd;
        }
    }


    public virtual void OnStart() {}


    public virtual void OnActivate() {}
    public virtual void OnDeactivate() {}


    public virtual void OnFixedUpdate() {}
    public virtual void OnISOFixedUpdate() {}

    public virtual void OnUpdate() {}
    public virtual void OnISOUpdate() {}

    public virtual void OnLateUpdate() {}
    public virtual void OnISOLateUpdate() {}

    public virtual void OnCollidedEnter(CustomCollider other) { }
    public virtual void OnCollidedStay(CustomCollider other) { }
    public virtual void OnCollidedEnd(CustomCollider other) { }
}
