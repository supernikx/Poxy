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
    /// Funzione di moviemento in stato di roaming
    /// </summary>
    void MoveRoaming();

    /// <summary>
    /// Funzione di movimento in stato di alert
    /// Se restituisce false, il player non è più in vista
    /// </summary>
    void AlertActions();

    /// <summary>
    /// Funzione che manda il nemico in allerta
    /// </summary>
    void Alert();

    /// <summary>
    /// Funzione che imposta il Path del nemico
    /// </summary>
    void SetPath();

    #region Parasite
    /// <summary>
    /// Funzione che "infesta" il nemico
    /// </summary>
    void Parasite(Player _player);

    /// <summary>
    /// Funzione che termina il parassita
    /// </summary>
    void EndParasite();
    #endregion

    #region Stun
    /// <summary>
    /// Funzione che aumenta di 1 i colpi stun ricevuti dal nemico
    /// </summary>
    void StunHit();

    /// <summary>
    /// Funzione che stunna il nemico
    /// </summary>
    void Stun();
    #endregion

    #region Damage
    /// <summary>
    /// Funzione che toglie al nemico i danni del proiettile
    /// </summary>
    void DamageHit(IBullet _bullet);

    /// <summary>
    /// Funzione che uccide il nemico
    /// </summary>
    void Die();
    #endregion

    #region Reset
    /// <summary>
    /// Funzione che reimposta i dati con i valori di default
    /// </summary>
    void ResetData();

    /// <summary>
    /// Funzione che reimposta la poszione del nemico con quella iniziale
    /// </summary>
    void ResetPosition();
    #endregion

    #region Getter
    /// <summary>
    /// Funzione che ritorna il danno del nemico
    /// </summary>
    /// <returns></returns>
    int GetDamage();

    /// <summary>
    /// Funzione che ritorna la direzione
    /// </summary>
    /// <returns></returns>
    int GetDirection();

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
    Collider GetCollider();

    /// <summary>
    /// Funzione che ritorna il Tollerance Bar Controller
    /// </summary>
    /// <returns></returns>
    EnemyToleranceController GetToleranceCtrl();

    /// <summary>
    /// Funzione che ritorna il Movement Controller
    /// </summary>
    /// <returns></returns>
    EnemyMovementController GetMovementCtrl();

    /// <summary>
    /// Funzione che ritorna il Collision Controller
    /// </summary>
    /// <returns></returns>
    EnemyCollisionController GetCollisionCtrl();

    /// <summary>
    /// Funzione che ritorna il View Controller
    /// </summary>
    /// <returns></returns>
    EnemyViewController GetViewCtrl();

    /// <summary>
    /// Funzione che ritorna il parent dei nemici
    /// </summary>
    /// <returns></returns>
    Transform GetEnemyDefaultParent();

    /// <summary>
    /// Funzione che ritorna il layer dei nemici
    /// </summary>
    /// <returns></returns>
    int GetEnemyDefaultLayer();
    #endregion
}
