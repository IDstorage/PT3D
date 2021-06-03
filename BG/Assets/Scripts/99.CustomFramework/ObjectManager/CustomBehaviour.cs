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
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.START);
        }
        if (IsOverriden(this.GetType().GetMethod("OnActivate")) || IsOverriden(this.GetType().GetMethod("OnDeactivate"))) {
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.ACTIVATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnFixedUpdate"))) {
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.FIXEDUPDATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnISOFixedUpdate"))) { 
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.FIXEDUPDATE_ISO);
        }
        if (IsOverriden(this.GetType().GetMethod("OnUpdate"))) {
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.UPDATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnISOUpdate"))) {
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.UPDATE_ISO);
        }
        if (IsOverriden(this.GetType().GetMethod("OnLateUpdate"))) {
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.LATEUPDATE);
        }
        if (IsOverriden(this.GetType().GetMethod("OnISOLateUpdate"))) {
            CustomFramework.ObjectManager.Register(this, ObjectManager.EFunctionType.LATEUPDATE_ISO);
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

}
