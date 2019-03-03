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
    public delegate void PlayeDamageableCollisionDelegate(IDamageable damageable);
    public PlayeDamageableCollisionDelegate OnDemageableCollision;
    public Action OnPlayerMaxHealth;
    public Action OnPlayerImmunityEnd;
    public Action OnPlayerDeath;
    #endregion

    [Header("General Settings")]
    [SerializeField]
    GameObject playerGraphic;
    GameObject graphic;

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
    /// <summary>
    /// Reference all'animation controller
    /// </summary>
    private PlayerLivesController livesCtrl;
    /// <summary>
    /// Reference all'animation controller
    /// </summary>
    private PlayerAnimationController animCtrl;

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init(EnemyManager _enemyMng, PlatformManager _platformMng)
    {
        //Prendo le referenze ai component e li inizializzo
        collisionCtrl = GetComponent<PlayerCollisionController>();
        if (collisionCtrl != null)
            collisionCtrl.Init(this);

        shootCtrl = GetComponent<PlayerShotController>();
        if (shootCtrl != null)
            shootCtrl.Init(this, PoolManager.instance);

        movementCtrl = GetComponent<PlayerMovementController>();
        if (movementCtrl != null)
            movementCtrl.Init(collisionCtrl);

        parasiteCtrl = GetComponent<PlayerParasiteController>();
        if (parasiteCtrl != null)
            parasiteCtrl.Init(this, _enemyMng, _platformMng);

        healthCtrl = GetComponent<PlayerHealthController>();
        if (healthCtrl != null)
            healthCtrl.Init(this);

        livesCtrl = GetComponent<PlayerLivesController>();
        if (livesCtrl != null)
            livesCtrl.Init();

        playerSM = GetComponent<PlayerSMController>();
        if (playerSM != null)
            playerSM.Init(this);

        animCtrl = GetComponentInChildren<PlayerAnimationController>();
        if (animCtrl != null)
            animCtrl.Init(collisionCtrl);

        //Setup cose locali
        graphic = playerGraphic;
    }

    /// <summary>
    /// Funzione che attiva la coroutine ParasiteEnemyCoroutine
    /// </summary>
    /// <param name="e"></param>
    public void StartParasiteEnemyCoroutine(IEnemy _e)
    {
        StartCoroutine(ParasiteEnemyCoroutine(_e));
    }
    /// <summary>
    /// Coroutine che manda il player in stato parassita rispetto al nemico
    /// </summary>
    /// <param name="_e"></param>
    /// <returns></returns>
    private IEnumerator ParasiteEnemyCoroutine(IEnemy _e)
    {
        parasiteCtrl.SetParasite(_e as IControllable);
        shootCtrl.SetCanShoot(false);
        shootCtrl.SetEnemyShot(_e.GetShotType());
        _e.Parasite(this);
        collisionCtrl.CheckDamageCollision(false);

        #region Animazione(per ora fatta a caso)
        Vector3 enemyPosition = _e.gameObject.transform.position;
        enemyPosition.z = transform.position.z;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOJump(enemyPosition, 1, 1, 0.5f)).Insert(0, graphic.transform.DOScale(0.5f, 0.5f));
        yield return sequence.WaitForCompletion();
        #endregion

        ChangeGraphics(_e.GetGraphics());
        animCtrl.SetAnimator(_e.GetAnimationController().GetAnimator());

        collisionCtrl.CheckDamageCollision(true);
        shootCtrl.SetCanShoot(true);
        if (playerSM.OnPlayerEnemyParaiste != null)
            playerSM.OnPlayerEnemyParaiste(_e as IControllable);
    }

    /// <summary>
    /// Funzione che attiva la coroutine ParasitePlatformCoroutine
    /// </summary>
    /// <param name="e"></param>
    public void StartParasitePlatformCoroutine(LaunchingPlatform _e)
    {
        StartCoroutine(ParasitePlatformCoroutine(_e));
    }
    /// <summary>
    /// Coroutine che manda il player in stato parassita rispetto alla piattaforma
    /// </summary>
    /// <param name="_e"></param>
    /// <returns></returns>
    private IEnumerator ParasitePlatformCoroutine(LaunchingPlatform _e)
    {
        parasiteCtrl.SetParasite(_e as IControllable);
        _e.Parasite(this);
        shootCtrl.SetCanShoot(false);
        collisionCtrl.CheckDamageCollision(false);

        #region Animazione(per ora fatta a caso)
        Vector3 platformPosition = _e.gameObject.transform.position;
        platformPosition.z = transform.position.z;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOJump(platformPosition, 1, 1, 0.5f)).Insert(0, graphic.transform.DOScale(0.5f, 0.5f));
        yield return sequence.WaitForCompletion();
        #endregion

        ChangeGraphics(_e.GetGraphics());
        collisionCtrl.CheckDamageCollision(true);
        if (playerSM.OnPlayerPlatformParaiste != null)
            playerSM.OnPlayerPlatformParaiste(_e);
    }

    /// <summary>
    /// Funzione che manda il player in normalstate
    /// </summary>
    public void GoToNormalState()
    {
        if (playerSM.OnPlayerNormal != null)
            playerSM.OnPlayerNormal();
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
        ChangeGraphics(playerGraphic);
        animCtrl.SetAnimator(animCtrl.GetPlayerAnimator());
        shootCtrl.ResetEnemyShot();
        switch (parasiteCtrl.GetParasite().GetControllableType())
        {
            case ControllableType.Enemy:
                movementCtrl.SetEject(1.3f);
                break;
            case ControllableType.Platform:
                movementCtrl.SetEject(1f);
                break;
        }

        parasiteCtrl.GetParasite().EndParasite();

        #region Animazione (per ora fatta a caso)
        graphic.transform.DOScale(1, 0.5f);
        yield return null;
        #endregion

        if (playerSM.OnPlayerNormal != null)
            playerSM.OnPlayerNormal();
    }

    /// <summary>
    /// Funzione che attiva la coroutine ImmunityCoroutine
    /// </summary>
    /// <param name="_immunityDuration"></param>
    public void StartImmunityCoroutine(float _immunityDuration)
    {
        StartCoroutine(ImmunityCoroutine(_immunityDuration));
    }
    /// <summary>
    /// Coroutine che rende il player immune e avvisa quando è finità l'immunità
    /// </summary>
    /// <param name="_immunityDuration"></param>
    /// <returns></returns>
    private IEnumerator ImmunityCoroutine(float _immunityDuration)
    {
        GetCollisionController().CheckDamageCollision(false);
        gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");
        float timer = _immunityDuration;
        while(timer > 0)
        {
            EnableGraphics(false);
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
            EnableGraphics(true);
            yield return new WaitForSeconds(0.2f);
            timer -= 0.2f;
        }

        GetCollisionController().CheckDamageCollision(true);
        gameObject.layer = LayerMask.NameToLayer("Player");
        if (OnPlayerImmunityEnd != null)
            OnPlayerImmunityEnd();
    }

    /// <summary>
    /// Funzione che attiva la coroutine DeathCoroutine
    /// </summary>
    public void StartDeathCoroutine()
    {
        StartCoroutine(DeathCoroutine());
    }
    /// <summary>
    /// Coroutine che manda il player in stato di morte
    /// </summary>
    /// <returns></returns>
    private IEnumerator DeathCoroutine()
    {
        if (OnPlayerDeath != null)
            OnPlayerDeath();
        yield return null;
    }

    /// <summary>
    /// Funzione che cambia la grafica con quella passata come parametro
    /// </summary>
    /// <param name="_newGraphic"></param>
    public void ChangeGraphics(GameObject _newGraphic)
    {
        EnableGraphics(false);
        graphic = _newGraphic;
        EnableGraphics(true);
    }

    /// <summary>
    /// Funzione che attiva/disattiva la grafica in base al parametro passato
    /// </summary>
    /// <param name="_switch"></param>
    public void EnableGraphics(bool _switch)
    {
        graphic.SetActive(_switch);
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

    public PlayerLivesController GetLivesController()
    {
        return livesCtrl;
    }
    /// <summary>
    /// Funzione che ritorna la grafica attiva del player
    /// </summary>
    /// <returns></returns>
    public GameObject GetActualGraphic()
    {
        return graphic;
    }
    #endregion
    #endregion
}
