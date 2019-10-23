using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class HelperScript : MonoBehaviour
    {
        [Range(-1, 1)]
        public float vertical;
        [Range(-1, 1)]
        public float horizontal;
        public string[] ohAttacks;
        public string[] thAttacks;
        public bool playAnim;
        public int weaponType;
        public bool enableRootMotion;
        public bool useItem;
        public bool interacting;
        public bool lockOn;
        Animator anim;
        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (useItem)
            {
                anim.Play("use_item");
                useItem = false;
            }
            enableRootMotion = !anim.GetBool("CanMove");
            anim.applyRootMotion = enableRootMotion;

            if (!lockOn)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }
            anim.SetBool("LockOn", lockOn);

            interacting = anim.GetBool("Interacting");
            if (enableRootMotion)
                return;
            if (interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical, -1.0f, 0.5f);
                horizontal = Mathf.Clamp(horizontal, -0.5f, 0.5f);
            }
            anim.SetInteger("WeaponType", weaponType);
            if (playAnim)
            {
                string targetAnim;
                if (weaponType == 1)
                {
                    int r = Random.Range(0, thAttacks.Length);
                    targetAnim = thAttacks[r];
                }
                else
                {
                    int r = Random.Range(0, ohAttacks.Length);
                    targetAnim = ohAttacks[r];
                }
                vertical = 0;
                anim.CrossFade(targetAnim, 0.2f);
                playAnim = false;
            }
            anim.SetFloat("Vertical", vertical);
            anim.SetFloat("Horizontal", horizontal);
        }
    }
}
