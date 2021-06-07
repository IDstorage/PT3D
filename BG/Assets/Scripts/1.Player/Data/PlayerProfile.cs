using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
#endif

[CreateAssetMenu(fileName = "Player Profile", menuName = "Data/Player Profile")]
public class PlayerProfile : ScriptableObject {
    public float MoveSpeed = 1F;
    public float JumpPower = 1F;
    public float GravityScale = 1F;

    public float MouseSpeedHorizontal = 1F;
    public float MouseSpeedVertical = 1F;
    public float PointDistance = 15F;
    public Vector2 CameraLimit = new Vector2(25F, 120F);
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerProfile))]
public class PlayerProfileEditor : Editor {

    PlayerProfile profile;

    static Dictionary<string, bool> foldCached = new Dictionary<string, bool>();

    public override void OnInspectorGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Save", GUILayout.Width(200), GUILayout.Height(30))) {
            EditorUtility.SetDirty(target);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(50);

        if (profile == null) profile = target as PlayerProfile;

        DisplayPlayerMovement();

        GUILayout.Space(20);

        DisplayCameraSetting();
    }

    bool MakeFolding(string title, float tabSize = 0F) {
        if (!foldCached.ContainsKey(title)) foldCached.Add(title, false);
        LeftTab(tabSize, () => {
            foldCached[title] = EditorGUILayout.Foldout(foldCached[title], title);
        });
        return foldCached[title];
    }

    void DisplayPlayerMovement() {
        bool ret = MakeFolding("Player Movement");

        if (!ret) return;

        LeftTab(15F, () => { 
            profile.MoveSpeed = EditorGUILayout.FloatField("└ Move Speed", profile.MoveSpeed);
            profile.JumpPower = EditorGUILayout.FloatField("└ Jump Power", profile.JumpPower);
            profile.GravityScale = EditorGUILayout.FloatField("└ Gravity Scale", profile.GravityScale);
        });
    }

    void DisplayCameraSetting() {
        bool ret = MakeFolding("Camera Setting");

        if (!ret) return;

        LeftTab(15F, () => {
            profile.MouseSpeedHorizontal = EditorGUILayout.FloatField("└ Mouse Horizontal", profile.MouseSpeedHorizontal);
            profile.MouseSpeedVertical = EditorGUILayout.FloatField("└ Mouse Vertical", profile.MouseSpeedVertical);
            profile.PointDistance = EditorGUILayout.FloatField("└ Point Distance", profile.PointDistance);
            profile.CameraLimit = EditorGUILayout.Vector2Field("└ Cam Limit", profile.CameraLimit);
        });
    }

    void LeftTab(float tabSize, System.Action callback) {
        GUILayout.BeginHorizontal();
        GUILayout.Space(tabSize);
        GUILayout.BeginVertical();
        callback?.Invoke();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

}
#endif