using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Eventi del PoolManager
/// </summary>
public class PoolManagerEvets{
    public delegate void Events(IPoolObject _gameObject);
}

/// <summary>
/// Enumeratore che definisce lo stato dell'oggetto
/// </summary>
public enum State
{
    InPool,
    InUse,
    Destroying,
}

/// <summary>
/// Interfaccia da usare sugli oggetti che si vogliono mettere
/// in pool
/// </summary>
public interface IPoolObject {
    GameObject ownerObject { get; set; }
    GameObject gameObject { get; }
    State CurrentState
    {
        get;
        set;
    }
    void Setup();
    event PoolManagerEvets.Events OnObjectSpawn;
    event PoolManagerEvets.Events OnObjectDestroy;
}
