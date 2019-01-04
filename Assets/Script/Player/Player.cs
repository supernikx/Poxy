using System;
using UnityEngine;
using StateMachine.PlayerSM;
using System.Collections;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    #region Delegates
    public delegate void PlayerEnemyCollisionDelegate(IEnemy _enemy);
    public PlayerEnemyCollisionDelegate OnEnemyCollision;
    public Action OnPlayerMaxHealth;
    #endregion
    [Header("General Settings")]
    [SerializeField]
    GameObject graphics;

    /// <summary>
    /// Riferimento al player controller
    /// </summary>
    private PlayerCollisionController collisionCtrl;
    /// <summary>
    /// Riferimento al shoot controller
    /// </summary>
    private PlayerShotController shootCtrl;
    /// <summary>
    /// Riferimento al player movement controller
    /// </summary>
    private PlayerMovementController movementCtrl;
    /// <summary>
    /// Riferimento al parasite controller
    /// </summary>
    private PlayerParasiteController parasiteCtrl;
    /// <summary>
    /// Riferimento alla state machine del player
    /// </summary>
    private PlayerSMController playerSM;
    /// <summary>
    /// Reference to Health Controller
    /// </summary>
    private PlayerHealthController healthCtrl;

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init(EnemyManager _enemyMng)
    {
        //Prendo le referenze ai component e li inizializzo
        collisionCtrl = GetComponent<PlayerCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init(this);

        shootCtrl = GetComponent<PlayerShotController>();
        if (shootCtrl != null)
            shootCtrl.Init(PoolManager.instance);

        movementCtrl = GetComponent<PlayerMovementController>();
        if (movementCtrl != null)
            movementCtrl.Init(collisionCtrl);

        parasiteCtrl = GetComponent<PlayerParasiteController>();
        if (parasiteCtrl != null)
            parasiteCtrl.Init(this, _enemyMng);

        healthCtrl = GetComponent<PlayerHealthController>();
        if (healthCtrl != null)
            healthCtrl.Init();

        playerSM = GetComponent<PlayerSMController>();
        if (playerSM != null)
            playerSM.Init(this);
    }

    /// <summary>
    /// Funzione che attiva la coroutine ParasiteCoroutine
    /// </summary>
    /// <param name="e"></param>
    public void StartParasiteCoroutine(IEnemy _e)
    {
        StartCoroutine(ParasiteCoroutine(_e));
    }
    /// <summary>
    /// Coroutine che manda il player in stato parassita rispetto al nemico
    /// </summary>
    /// <param name="_e"></param>
    /// <returns></returns>
    private IEnumerator ParasiteCoroutine(IEnemy _e)
    {
        parasiteCtrl.SetParasiteEnemy(_e);
        _e.Parasite(this);
        collisionCtrl.CheckDamageCollision(false);

        #region Animazione(per ora fatta a caso)
        Vector3 enemyPosition = _e.gameObject.transform.position;
        enemyPosition.z = transform.position.z;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOJump(enemyPosition, 1, 1, 0.5f)).Insert(0, graphics.transform.DOScale(0.5f, 0.5f));
        yield return sequence.WaitForCompletion();
        #endregion

        collisionCtrl.CheckDamageCollision(true);
        if (playerSM.OnPlayerParaiste != null)
            playerSM.OnPlayerParaiste(_e);
    }

    /// <summary>
    /// Funzione che attiva la coroutine NormalCoroutine
    /// </summary>
    public void StartNormalCoroutine()
    {
        StartCoroutine(NormalCoroutine());
    }
    /// <summary>
    /// Funzione che manda il player nello stato normale
    /// </summary>
    private IEnumerator NormalCoroutine()
    {
        parasiteCtrl.GetParasiteEnemy().EndParasite();
        movementCtrl.Eject();

        #region Animazione (per ora fatta a caso)
        graphics.transform.DOScale(1, 0.5f);
        yield return null;
        #endregion

        if (playerSM.OnPlayerNormal != null)
            playerSM.OnPlayerNormal();
    }

    /// <summary>
    /// Funzione che attiva/disattiva la grafica in base al parametro passato
    /// </summary>
    /// <param name="_switch"></param>
    public void EnableGraphics(bool _switch)
    {
        graphics.SetActive(_switch);
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna lo shoot controller
    /// </summary>
    /// <returns></returns>
    public PlayerShotController GetShotController()
    {
        return shootCtrl;
    }
    /// <summary>
    /// Funzione che ritorna il collision controller
    /// </summary>
    /// <returns></returns>
    public PlayerCollisionController GetCollisionController()
    {
        return collisionCtrl;
    }
    /// <summary>
    /// Funzione che ritorna il movement controller
    /// </summary>
    /// <returns></returns>
    public PlayerMovementController GetMovementController()
    {
        return movementCtrl;
    }
    /// <summary>
    /// Function that returns the health controller
    /// </summary>
    /// <returns></returns>
    public PlayerHealthController GetHealthController()
    {
        return healthCtrl;
    }
    /// <summary>
    /// Funzione che ritorna il parasite controller
    /// </summary>
    /// <returns></returns>
    public PlayerParasiteController GetParasiteController()
    {
        return parasiteCtrl;
    }
    #endregion
    #endregion
}
