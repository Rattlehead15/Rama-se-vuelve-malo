using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class AnimatorHook : MonoBehaviour
    {
        Animator anim;
        StateManager states;
        Rigidbody rigid;
        EnemyStates eStates;
        public float rootMotionMultiplier;
        bool rolling;
        float roll_t;
        float delta;
        AnimationCurve roll_curve;


        public void Init(StateManager sm, EnemyStates est)
        {
            states = sm;
            eStates = est;
            if (states != null)
            {
                anim = sm.anim;
                rigid = sm.body;
                delta = sm.delta;
                roll_curve = sm.rollCurve;
            }
            if (eStates != null)
            {
                anim = est.anim;
                rigid = est.body;
                delta = est.delta;
            }
        }

        public void InitForRoll()
        {
            if (!rolling)
            {
                rolling = true;
                roll_t = 0;
            }
        }

        public void CloseRoll()
        {
            if (!rolling)
                return;
            rootMotionMultiplier = 1;
            roll_t = 0;
            rolling = false;
        }

        private void OnAnimatorMove()
        {
            if (states == null && eStates == null)
                return;

            if (rigid == null)
                return;

            if(states != null)
            {
                if (states.canMove)
                    return;
                delta = states.delta;
            }
            if(eStates != null)
            {
                if (eStates.canMove)
                    return;
                delta = eStates.delta;
            }


            rigid.drag = 0;

            if(rootMotionMultiplier == 0)
            {
                rootMotionMultiplier = 1;
            }

            if (!rolling)
            {
                Vector3 vdelta = anim.deltaPosition;
                vdelta.y = 0;
                Vector3 v = (vdelta * rootMotionMultiplier) / delta;
                rigid.velocity = v;
            }
            else
            {
                roll_t += delta / 0.4f;
                if(roll_t > 1)
                {
                    roll_t = 1;
                }

                if (states == null)
                    return;

                float zValue = roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rootMotionMultiplier);
                rigid.velocity = v2;
            }
        }
        public void OpenDamageColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.CloseAllDamageColliders();
        }
        public void CloseDamageColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.CloseAllDamageColliders();
        }
    }

}
