using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class ButtonBase : MonoBehaviour, IButton
{
    [Header("Button Options")]
    [SerializeField]
    protected ButtonTriggerType triggerType;
    [SerializeField]
    protected List<GameObject> targets = new List<GameObject>();

    [Header("Graphic Settings")]
    [SerializeField]
    protected Material activeMaterial;
    [SerializeField]
    protected Material inactiveMaterial;

    new protected Renderer renderer;
    protected bool isActive;

    #region Abstract
    public virtual void Init()
    {
        renderer = GetComponentInChildren<Renderer>();
    }
    public virtual void Setup()
    {
        isActive = true;
        renderer.material = activeMaterial;
    }
    public virtual void Activate()
    {
        if (!isActive)
            return;

        isActive = false;
        renderer.material = inactiveMaterial;
    }
    #endregion

    #region Getters
    public virtual ButtonTriggerType GetTriggerType()
    {
        return triggerType;
    }
    #endregion
}
