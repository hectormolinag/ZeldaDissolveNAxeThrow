using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : MonoBehaviour
{
    [SerializeField] GameObject obj = null;
    [SerializeField] PlayerController player = null;

    void DeactivateCamera()
    {
        player.ReActivatePlayerMovement();
        obj.SetActive(false);
    }
}
