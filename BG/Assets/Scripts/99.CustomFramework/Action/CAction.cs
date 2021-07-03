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

        public bool isPlaying = false;
        public bool isPaused = false;
        public bool isLoop = false;

        public float duration;

        public static Dictionary<EEaseAction, AnimationCurve> curveDictionary = new Dictionary<EEaseAction, AnimationCurve>();

        public static float GetEaseScale(EEaseAction ease, float threshold) {
            return curveDictionary[ease].Evaluate(threshold);
        }


        public System.Action onStart, onBeforeStep, onAfterStep, onComplete;


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

        protected float timeCount = 0F;

        public virtual void Init() { timeCount = 0F; isPaused = false; }
        public virtual bool Execute(float dt) { return true; }
        public virtual void SmoothComplete() { }

        public CAction SetLoop(bool loop = false) {
            isLoop = loop;
            return this;
        }

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
            isPlaying = false;
        }

        public static void Stop(CAction act) {
            act.Stop();
        }

        void Play() {
            isPlaying = true;
            CActionMono.Instance.AddChild(this);
        }

        public static void Play(CAction act) {
            act.Play();
        }
    }

    public class CMoveTo : CAction {
        Transform target;

        Vector3 startPosition, endPosition;
        EEaseAction easeAction;

        public static CMoveTo Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CMoveTo act = new CMoveTo();
            act.target = target;
            act.endPosition = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startPosition = target.transform.position;
        }
        public override bool Execute(float dt) {
            target.transform.position = Vector3.Lerp(startPosition, endPosition, GetEaseScale(easeAction, timeCount / duration));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.position = endPosition;
        }
    }

    public class CMoveBy : CAction {
        Transform target;

        Vector3 startPosition, endPosition, deltaVector;
        EEaseAction easeAction;

        public static CMoveBy Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CMoveBy act = new CMoveBy();
            act.target = target;
            act.deltaVector = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startPosition = target.transform.position;
            endPosition = startPosition + deltaVector;
        }
        public override bool Execute(float dt) {
            target.transform.position = Vector3.Lerp(startPosition, endPosition, GetEaseScale(easeAction, timeCount / duration));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.position = endPosition;
        }
    }

    public class CLocalMoveTo : CAction {
        Transform target;

        Vector3 startPosition, endPosition;
        EEaseAction easeAction;

        public static CLocalMoveTo Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CLocalMoveTo act = new CLocalMoveTo();
            act.target = target;
            act.endPosition = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startPosition = target.transform.localPosition;
        }
        public override bool Execute(float dt) {
            target.transform.localPosition = Vector3.Lerp(startPosition, endPosition, GetEaseScale(easeAction, timeCount / duration));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.localPosition = endPosition;
        }
    }

    public class CLocalMoveBy : CAction {
        Transform target;

        Vector3 startPosition, endPosition, deltaVector;
        EEaseAction easeAction;

        public static CLocalMoveBy Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CLocalMoveBy act = new CLocalMoveBy();
            act.target = target;
            act.deltaVector = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startPosition = target.transform.localPosition;
            endPosition = startPosition + deltaVector;
        }
        public override bool Execute(float dt) {
            target.transform.localPosition = Vector3.Lerp(startPosition, endPosition, GetEaseScale(easeAction, timeCount / duration));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.localPosition = endPosition;
        }
    }

    public class CRotateTo : CAction {
        Transform target;

        Vector3 startAngle, endAngle;
        EEaseAction easeAction;

        public static CRotateTo Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CRotateTo act = new CRotateTo();
            act.target = target;
            act.endAngle = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startAngle = target.transform.eulerAngles;
        }
        public override bool Execute(float dt) {
            target.transform.rotation = Quaternion.Euler(Vector3.Lerp(startAngle, endAngle, GetEaseScale(easeAction, timeCount / duration)));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.rotation = Quaternion.Euler(endAngle);
        }
    }

    public class CRotateBy : CAction {
        Transform target;

        Vector3 startAngle, endAngle, deltaAngle;
        EEaseAction easeAction;

        public static CRotateBy Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CRotateBy act = new CRotateBy();
            act.target = target;
            act.deltaAngle = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startAngle = target.transform.eulerAngles;
            endAngle = startAngle + deltaAngle;
        }
        public override bool Execute(float dt) {
            target.transform.rotation = Quaternion.Euler(Vector3.Lerp(startAngle, endAngle, GetEaseScale(easeAction, timeCount / duration)));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.rotation = Quaternion.Euler(endAngle);
        }
    }

    public class CLocalRotateTo : CAction {
        Transform target;

        Vector3 startAngle, endAngle;
        EEaseAction easeAction;

        public static CLocalRotateTo Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CLocalRotateTo act = new CLocalRotateTo();
            act.target = target;
            act.endAngle = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startAngle = target.transform.localEulerAngles;
        }
        public override bool Execute(float dt) {
            target.transform.localRotation = Quaternion.Euler(Vector3.Lerp(startAngle, endAngle, GetEaseScale(easeAction, timeCount / duration)));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.localRotation = Quaternion.Euler(endAngle);
        }
    }

    public class CLocalRotateBy : CAction {
        Transform target;

        Vector3 startAngle, endAngle, deltaAngle;
        EEaseAction easeAction;

        public static CLocalRotateBy Create(Transform target, Vector3 end, float time, EEaseAction ease = EEaseAction.LINEAR) {
            CLocalRotateBy act = new CLocalRotateBy();
            act.target = target;
            act.deltaAngle = end;
            act.duration = time;
            act.easeAction = ease;
            return act;
        }

        public override void Init() {
            base.Init();
            startAngle = target.transform.localEulerAngles;
            endAngle = startAngle + deltaAngle;
        }
        public override bool Execute(float dt) {
            target.transform.localRotation = Quaternion.Euler(Vector3.Lerp(startAngle, endAngle, GetEaseScale(easeAction, timeCount / duration)));
            timeCount += dt;
            return timeCount > duration;
        }
        public override void SmoothComplete() {
            base.SmoothComplete();
            target.transform.localRotation = Quaternion.Euler(endAngle);
        }
    }


    public class CCallFunc : CAction {
        public static CCallFunc Create(System.Action callback) {
            CCallFunc act = new CCallFunc();
            act.onComplete = callback;
            return act;
        }

        public new CAction OnComplete(System.Action callback) {
            return this;    // Block onComplete assignment
        }

        public override bool Execute(float dt) {
            return true;
        }
    }

    public class CDelay : CAction {
        public static CDelay Create(float delay) {
            CDelay act = new CDelay();
            act.duration = delay;
            return act;
        }

        public override bool Execute(float dt) {
            timeCount += dt;
            return timeCount > duration;
        }
    }


    public class CSequence : CAction {
        class SequenceUnit {
            public float delay;
            public CAction action;
        }

        List<SequenceUnit> actionList = new List<SequenceUnit>();

        float globalDelay = 0F;

        public static CSequence Create(params CAction[] actions) {
            CSequence act = new CSequence();
            float globalDelay = 0F;
            for (int i = 0; i < actions.Length; ++i) {
                act.actionList.Add(new SequenceUnit() {
                    delay = globalDelay,
                    action = actions[i]
                });
                globalDelay += actions[i].duration;
            }
            return act;
        }


        public CSequence Clear() {
            actionList.Clear();
            onStart = null;
            onBeforeStep = null;
            onAfterStep = null;
            onComplete = null;
            timeCount = 0F;
            globalDelay = 0F;
            return this;
        }


        public CSequence Append(CAction action) {
            if (isPlaying) return null; // To make error;

            float _delay = 0F;
            if (actionList.Count > 0) {
                _delay = actionList[actionList.Count - 1].delay + actionList[actionList.Count - 1].action.duration;
            }

            actionList.Add(new SequenceUnit() {
                delay = _delay,
                action = action
            });
            return this;
        }

        public CSequence AppendInterval(float interval) {
            if (isPlaying) return null;

            return Append(CDelay.Create(interval));
        }

        public CSequence AppendCallback(System.Action callback) {
            if (isPlaying) return null;

            return Append(CCallFunc.Create(callback));
        }

        public CSequence Join(CAction action) {
            if (isPlaying) return null;
            actionList.Add(new SequenceUnit() {
                delay = actionList[actionList.Count - 1].delay,
                action = action
            });
            return this;
        }

        public CSequence Insert(float time, CAction action) {
            if (isPlaying) return null;
            actionList.Add(new SequenceUnit() {
                delay = time,
                action = action
            });
            return this;
        }

        public override void Init() {
            base.Init();
            for (int i = 0; i < actionList.Count; ++i) {
                if (globalDelay > actionList[i].delay + actionList[i].action.duration) continue;
                globalDelay = actionList[i].delay + actionList[i].action.duration;
            }
        }
        public override bool Execute(float dt) {
            for (int i = 0; i < actionList.Count; ++i) {
                if (actionList[i].delay <= 0F) {
                    Play(actionList[i].action);
                    actionList.RemoveAt(i--);
                    continue;
                }
                actionList[i].delay -= dt;
            }

            timeCount += dt;
            return timeCount > globalDelay;
        }
    }

}