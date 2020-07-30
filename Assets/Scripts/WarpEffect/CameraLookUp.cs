using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookUp : MonoBehaviour
{
    public bool canLookUp = false;
    float t;

    void Start()
    {
        t = Time.deltaTime / 3;
    }
    void Update()
    {
        if (canLookUp)
            transform.Rotate(new Vector3(-0.15f, 0, 0));
    }
}
