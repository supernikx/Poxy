using UnityEngine;
using System.Collections;
using System;

public interface IButton
{
    //event EventHandler ActivateEvent;

    void Init();
    void Activate();
    ButtonTriggerType GetTriggerType();
}

public enum ButtonTriggerType
{
    Shot,
    Collision,
    Enemy
}
