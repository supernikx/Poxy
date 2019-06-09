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
    /// Funzione che segue il knockback
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="_force"></param>
    void ApplyKnockback(Vector3 _dir, float _force);

    #region Shot
    /// <summary>
    /// Funzione che controlla se puoi sparare e ritorna true o false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    bool CheckShot(Transform _target);

    /// <summary>
    /// Funzione che controlla se sei in range di sparo e ritorna true o false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    bool CheckRange(Transform _target);

    /// <summary>
    /// Funzione che fa sparare il nemico e ritorna true se spara, altrimenti false
    /// </summary>
    /// <param name="_target"></param>
    /// <returns></returns>
    bool Shot(Transform _target);
    #endregion

    #region StateHandler
    /// <summary>
    /// Funzione che avvisa l'ingresso in roaming state
    /// </summary>
    void EnemyRoamingState();
    /// <summary>
    /// Funzione che avvisa l'ingresso in alert state
    /// </summary>
    void EnemyAlertState();
    #endregion

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
    void DamageHit(float _damage);

    /// <summary>
    /// Funzione che uccide il nemico
    /// </summary>
    void Die(float _respawnTime = -1);
    #endregion

    #region Reset
    /// <summary>
    /// Funzione che reimposta i dati con i valori di default
    /// </summary>
    void ResetLife();

    /// <summary>
    /// Funzione che reimposta la poszione del nemico con quella iniziale
    /// </summary>
    void ResetPosition();

    /// <summary>
    /// Funzione che reimposta gli stunhit
    /// </summary>
    void ResetStunHit();

    /// <summary>
    /// Funzione che riporta il nemico in stato di roaming
    /// </summary>
    void Respawn();
    #endregion

    #region Getter
    /// <summary>
    /// Funzione che ritorna il tipo di proiettile del nemico
    /// </summary>
    /// <returns></returns>
    ObjectTypes GetBulletType();

    /// <summary>
    /// Funzione che ritorna lo shot point del nemico
    /// </summary>
    /// <returns></returns>
    Transform GetShotPoint();

    /// <summary>
    /// Funzione che ritorna la vita del nemico
    /// </summary>
    /// <returns></returns>
    float GetHealth();

    /// <summary>
    /// Funzione che ritorna la movement speed del nemcio
    /// </summary>
    /// <returns></returns>
    float GetMovementSpeed();

    /// <summary>
    /// Funzione che ritorna la lunghezza del path
    /// </summary>
    /// <returns></returns>
    float GetPathLenght();

    /// <summary>
    /// Funzione che ritorna il danno del nemico
    /// </summary>
    /// <returns></returns>
    float GetDamage();

    /// <summary>
    /// Get Stun Duration
    /// </summary
    float GetStunDuration();

    /// <summary>
    /// Get Death Duration
    /// </summary>
    float GetRespawnDuration();

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    IGraphic GetGraphics();

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
    /// Funzione che ritorna l'animation controller
    /// </summary>
    /// <returns></returns>
    EnemyAnimationController GetAnimationController();

    /// <summary>
    /// Funzione che ritorna il vfx controller del nemico
    /// </summary>
    /// <returns></returns>
    EnemyVFXController GetVFXController();

    /// <summary>
    /// Funzione che ritorna il command sprite controller
    /// </summary>
    /// <returns></returns>
    EnemyCommandsSpriteController GetEnemyCommandsSpriteController();

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

    #region Setter
    /// <summary>
    /// Funzione che imposta se il nemico può essere stunnato
    /// </summary>
    /// <param name="_switc"></param>
    void SetCanStun(bool _switc);

    /// <summary>
    /// Funzione che imposta se il nemico può prendere danno
    /// </summary>
    /// <param name="_switc"></param>
    void SetCanTakeDamage(bool _canTakeDamage);
    #endregion
}
