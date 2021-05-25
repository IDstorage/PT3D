using System.Collections.Generic;
using UnityEngine;

namespace CustomFramework {

    public enum EEaseAction {
        LINEAR,

        IN,
        INOUT,
        OUT,

        IN_CUBIC,
        INOUT_CUBIC,
        OUT_CUBIC
    }

    public struct EEaseActionExt : IEqualityComparer<EEaseAction> {
        public bool Equals(EEaseAction a, EEaseAction b) {
            return a == b;
        }

        public int GetHashCode(EEaseAction a) {
            return (int)a;
        }
    }

    /*
     * Action act = MoveTo.Create(obj).OnStart(...).OnComplete(...);
     * Sequence seq = Sequence.Create(act1, act2, act3, ...);
     * seq.Append(act4);
     * seq.Insert(act5);
     * 
     * obj.RunAction(act);
     * Action.Play(act);
     * 
     */

    public class CAction {

        string actionID;
        bool isPaused = false;

        static Dictionary<EEaseAction, AnimationCurve> curveDictionary;

        public static float GetEaseScale(EEaseAction ease, float threshold) {
            return curveDictionary[ease].Evaluate(threshold);
        }


        System.Action onStart, onBeforeStep, onAfterStep, onComplete;


        public CAction OnStart(System.Action callback) {
            this.onStart = callback;
            return this;
        }
        public CAction OnBeforeStep(System.Action callback) {
            this.onBeforeStep = callback;
            return this;
        }
        public CAction OnAfterStep(System.Action callback) {
            this.onAfterStep = callback;
            return this;
        }
        public CAction OnComplete(System.Action callback) {
            this.onComplete = callback;
            return this;
        }

        public virtual bool Execute(CustomBehaviour target, float dt) { return true; }
        public virtual void SmoothComplete(CustomBehaviour target) { }



        void Pause() {
            isPaused = true;
        }

        public static void Pause(CAction act) {
            act.Pause();
        }

        void Resume() {
            isPaused = false;
        }

        public static void Resume(CAction act) {
            act.Resume();
        }

        void Stop() {

        }

        public static void Stop(CAction act) {
            act.Stop();
        }

        void Play() {

        }

        public static void Play(CAction act) {
            act.Play();
        }
    }

    public class CMoveTo : CAction {
        Vector3 startPosition, endPosition;
        float duration;
        EEaseAction easeAction;

        public static CMoveTo Create(Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CMoveTo act = new CMoveTo();
            act.endPosition = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }


        float timeCount = 0F;

        public override bool Execute(CustomBehaviour target, float dt) {
            if (timeCount < dt) startPosition = target.transform.position;
            target.transform.position = Vector3.Lerp(startPosition, endPosition, GetEaseScale(easeAction, timeCount / duration));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete(CustomBehaviour target) {
            target.transform.position = endPosition;
        }
    }

    public class CMoveBy : CAction {
        Vector3 startPosition, endPosition, deltaVector;
        float duration;
        EEaseAction easeAction;

        public static CMoveBy Create(Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CMoveBy act = new CMoveBy();
            act.deltaVector = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }


        float timeCount = 0F;

        public override bool Execute(CustomBehaviour target, float dt) {
            if (timeCount < dt) {
                startPosition = target.transform.position;
                endPosition = startPosition + deltaVector;
            }
            target.transform.position = Vector3.Lerp(startPosition, endPosition, GetEaseScale(easeAction, timeCount / duration));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete(CustomBehaviour target) {
            target.transform.position = endPosition;
        }
    }

}