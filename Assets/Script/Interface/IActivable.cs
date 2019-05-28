using UnityEngine;
using System.Collections;
using System;


public interface IActivable
{
    //event EventHandler ActivateEvent;

    void Init();
    void Setup();
    void Activate();

}
