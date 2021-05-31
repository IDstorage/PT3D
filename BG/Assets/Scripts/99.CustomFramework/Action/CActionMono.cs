using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomFramework {

    public class CActionMono : CustomBehaviour {

        static CActionMono instance = null;
        public static CActionMono Instance {
            get {
                if (instance == null) {
                    instance = GameObject.FindObjectOfType<CActionMono>();
                }
                return instance;
            }
        }

        List<CAction> childList = new List<CAction>();

        [SerializeField] AnimationCurve[] curves;

        public override void OnStart() {
            DontDestroyOnLoad(this);
            for (int i = 0; i < curves.Length; ++i) {
                CAction.curveDictionary.Add((EEaseAction)i, curves[i]);
            }
        }

        public void AddChild(CAction act) {
            childList.Add(act);
        }

        public override void OnUpdate() {
            for (int i = 0; i < childList.Count; ++i) {
                childList[i].onBeforeStep?.Invoke();
                if (!childList[i].Execute(Time.deltaTime)) {
                    childList[i].onAfterStep?.Invoke();
                    continue;
                }
                childList[i].onAfterStep?.Invoke();
                childList[i].SmoothComplete();
                childList[i].onComplete?.Invoke();
                childList[i].isPlaying = false;
                childList.RemoveAt(i--);
            }
        }

    }

}