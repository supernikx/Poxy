using UnityEngine;
using System.Collections;
using StateMachine.EnemySM;

public interface IEnemy
{
    /// <summary>
    /// Funzione di inizializzazione
    /// </summary>
    void Init(EnemyManager _enemyMng);

    /// <summary>
    /// Referenza al gameObject
    /// </summary>
    GameObject gameObject { get;}

    /// <summary>
    /// Get Stun Duration
    /// </summary
    int GetStunDuration();

    /// <summary>
    /// Get Death Duration
    /// </summary>
    int GetDeathDuration();

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    GameObject GetGraphics();

    /// <summary>
    /// Get Collider Reference
    /// </summary>
    BoxCollider GetCollider();

    /// <summary>
    /// Funzione di moviemento
    /// </summary>
    /// <returns></returns>
    void Move();

    /// <summary>
    /// Funzione che stunna il nemico
    /// </summary>
    void Stun();

    /// <summary>
    /// Funzione che uccide il nemico
    /// </summary>
    void Die();

    /// <summary>
    /// Funzione che "infesta" il nemico
    /// </summary>
    void Parasite(Player _player);
}
