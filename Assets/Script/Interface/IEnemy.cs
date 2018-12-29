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

    int GetDamage();

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
    CapsuleCollider GetCollider();

    EnemyToleranceController GetToleranceCtrl();

    EnemyMovementController GetMovementCtrl();

    EnemyCollisionController GetCollisionCtrl();

    void SetPath();

    /// <summary>
    /// Funzione di moviemento in stato di roaming
    /// </summary>
    void MoveRoaming();

    /// <summary>
    /// Funzione di movimento in stato di alert
    /// </summary>
    void MoveAlert();

    /// <summary>
    /// Funzione di stop moviemento
    /// </summary>
    /// <returns></returns>
    void Stop();

    /// <summary>
    /// Funzione che stunna il nemico
    /// </summary>
    void Stun();

    /// <summary>
    /// Funzione che uccide il nemico
    /// </summary>
    void Die();

    /// <summary>
    /// Funzione che manda il nemico in allerta
    /// </summary>
    void Alert();

    /// <summary>
    /// Funzione che "infesta" il nemico
    /// </summary>
    void Parasite(Player _player);

    /// <summary>
    /// Funzione che termina il parassita
    /// </summary>
    void EndParasite();
}
