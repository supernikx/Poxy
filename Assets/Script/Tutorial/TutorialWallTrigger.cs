using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWallTrigger : MonoBehaviour
{
    #region Action
    public Action OnWallTriggered;
    #endregion

    public void Enable(bool _enable)
    {
        gameObject.SetActive(_enable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            if (OnWallTriggered != null)
                OnWallTriggered();
        }
    }
}
