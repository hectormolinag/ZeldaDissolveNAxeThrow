using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] Animator anim = null;
    [SerializeField] string nameOfTriggerAnim = "";


    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Axe"))
        {
            anim.SetTrigger(nameOfTriggerAnim);
        }
    }
}
