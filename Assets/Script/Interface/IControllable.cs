using UnityEngine;
using System.Collections;

public interface IControllable
{
    /// <summary>
    /// Referenza al gameObject
    /// </summary>
    GameObject gameObject { get; }

    ControllableType GetControllableType();

    void EndParasite();
}

public enum ControllableType
{
    Enemy,
    Platform,
}

