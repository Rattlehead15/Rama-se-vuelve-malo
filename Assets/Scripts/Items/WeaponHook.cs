using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class WeaponHook : MonoBehaviour
    {
        public GameObject[] damageColliders;
        public void OpenDamageColliders()
        {
            for (int i = 0; i < damageColliders.Length; i++)
            {
                damageColliders[i].SetActive(true);
            }
        }
        public void CloseDamageColliders()
        {
            for (int i = 0; i < damageColliders.Length; i++)
            {
                damageColliders[i].SetActive(false);
            }
        }
    }
}