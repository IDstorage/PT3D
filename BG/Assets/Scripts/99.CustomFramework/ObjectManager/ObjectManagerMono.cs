using UnityEngine;
using CustomFramework;

public class ObjectManagerMono : MonoBehaviour {

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AfterSceneLoaded() {
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Start() {
        DontDestroyOnLoad(gameObject);
    }


    void FixedUpdate() {
        ObjectManager.FixedUpdate();
    }

    void Update() {
        ObjectManager.Update();
    }

    void LateUpdate() {
        ObjectManager.LateUpdate();
    }
}