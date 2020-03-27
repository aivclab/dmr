using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Packages.Unity.ConstraintSamples.Scripts {
    public class mParent : MonoBehaviour
    {
        public GameObject mParentCon;

        enum Mode
        {
            Idle,
            Ground,
            Hand,
            Back
        }

        Mode m_Mode;

        public void Update()
        {
            if (this.m_Mode != Mode.Idle)
            {
                var constraint = this.mParentCon.GetComponent<MultiParentConstraint>();
                var sourceObjects = constraint.data.sourceObjects;

                sourceObjects.SetWeight(0, weight : this.m_Mode == Mode.Ground ? 1f : 0f);
                sourceObjects.SetWeight(1, weight : this.m_Mode == Mode.Hand ? 1f : 0f);
                sourceObjects.SetWeight(2, weight : this.m_Mode == Mode.Back ? 1f : 0f);
                constraint.data.sourceObjects = sourceObjects;

                this.m_Mode = Mode.Idle;
            }
        }

        public void Start()
        {
            this.m_Mode = Mode.Ground;
            Debug.Log ("ground");
        }
        public void hand()
        {
            this.m_Mode = Mode.Hand;
            Debug.Log ("hand");
        }

        public void back()
        {
            this.m_Mode = Mode.Back;
            Debug.Log ("back");
        }
    }
}
