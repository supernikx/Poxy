using UnityEngine;
using System;
using System.Collections;

public class FallingTrigger : MonoBehaviour
{
    public Action FallTriggerEvent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && FallTriggerEvent != null)
        {
            FallTriggerEvent();
        }
    }
}
