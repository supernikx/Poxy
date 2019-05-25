using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private GameObject triggersContainer;

    #region API
    public void Init()
    {
        foreach (TutorialTrigger trigger in triggersContainer.GetComponentsInChildren<TutorialTrigger>())
        {
            trigger.Init();
        }
    }
    #endregion
}
