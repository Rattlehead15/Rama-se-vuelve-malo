using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool isInvincible;
        public bool canMove;
        public bool isDead;
        public Animator anim;
        public Rigidbody body;
        EnemyTarget enemyTarget;
        AnimatorHook aHook;
        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();
        public float delta;
        private void Start()
        {
            health = 100;
            anim = GetComponentInChildren<Animator>();
            enemyTarget = GetComponent<EnemyTarget>();
            enemyTarget.Init(this);

            body = GetComponent<Rigidbody>();

            aHook = anim.GetComponent<AnimatorHook>();
            if (aHook == null)
                aHook = anim.gameObject.AddComponent<AnimatorHook>();
            aHook.Init(null, this);
            InitRagdoll();
        }

        public void InitRagdoll()
        {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if (rigs[i] == body)
                    continue;
                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagdoll()
        {
            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;
            }

            Collider controllerCollider = body.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            body.isKinematic = true;

            StartCoroutine("CloseAnimator");
        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForEndOfFrame();
            anim.enabled = false;
            this.enabled = false;
        }

        public void DoDamage(float v)
        {
            if (isInvincible)
                return;
            isInvincible = true;
            health -= v;
            anim.Play("damage_1");
            anim.applyRootMotion = true;
        }

        private void Update()
        {
            delta = Time.deltaTime;
            canMove = anim.GetBool("CanMove");

            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    EnableRagdoll();
                }
            }

            if (isInvincible)
            {
                isInvincible = !canMove;
            }

            if(canMove)
            {
                anim.applyRootMotion = false;
            }
        }
    }
}
