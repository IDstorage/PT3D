using UnityEngine;

namespace CustomFramework.Extension {

    public static class CActionExt {

        public static void MoveTo(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CMoveTo.Create(self, end, duration, ease).SetLoop(setLoop));
        }

        public static void MoveBy(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CMoveBy.Create(self, end, duration, ease).SetLoop(setLoop));
        }

        public static void LocalMoveTo(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CLocalMoveTo.Create(self, end, duration, ease).SetLoop(setLoop));
        }

        public static void LocalMoveBy(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CLocalMoveBy.Create(self, end, duration, ease).SetLoop(setLoop));
        }

        public static void RotateTo(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CRotateTo.Create(self, end, duration, ease).SetLoop(setLoop));
        }

        public static void RotateBy(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CRotateBy.Create(self, end, duration, ease).SetLoop(setLoop));
        }
        public static void LocalRotateTo(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CLocalRotateTo.Create(self, end, duration, ease).SetLoop(setLoop));
        }

        public static void LocalRotateBy(this Transform self, Vector3 end, float duration, bool setLoop = false, EEaseAction ease = EEaseAction.LINEAR) {
            CAction.Play(CLocalRotateBy.Create(self, end, duration, ease).SetLoop(setLoop));
        }

    }

}
