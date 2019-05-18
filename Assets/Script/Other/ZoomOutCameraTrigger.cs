using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomOutCameraTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            if (CameraManager.OnZoomOutTriggered != null)
                CameraManager.OnZoomOutTriggered(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            if (CameraManager.OnZoomOutTriggered != null)
                CameraManager.OnZoomOutTriggered(false);
        }
    }
}
