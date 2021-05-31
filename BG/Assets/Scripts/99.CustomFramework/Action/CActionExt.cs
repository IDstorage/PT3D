using UnityEngine;

namespace CustomFramework.Extension {

    public static class CActionExt {

        public static void MoveTo(this Transform self, Vector3 end, float duration, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CMoveTo.Create(self, end, duration, ease));
        }

        public static void MoveBy(this Transform self, Vector3 end, float duration, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CMoveBy.Create(self, end, duration, ease));
        }

        public static void RotateTo(this Transform self, Vector3 end, float duration, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CRotateTo.Create(self, end, duration, ease));
        }

        public static void RotateBy(this Transform self, Vector3 end, float duration, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CRotateBy.Create(self, end, duration, ease));
        }
    }

}
