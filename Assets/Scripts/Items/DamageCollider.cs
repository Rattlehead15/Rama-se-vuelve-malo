using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SA
{
    public class DamageCollider : MonoBehaviour
    {
        bool isColliding;
        private void OnTriggerEnter(Collider other)
        {
            EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();
            if (eStates == null)
                return;
            if(other.isTrigger == false)
                eStates.DoDamage(35);
        }
    }
}