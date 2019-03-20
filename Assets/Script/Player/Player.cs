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
    public PlayeDamageableCollisionDelegate OnDamageableCollision;
    public Action OnPlayerMaxHealth;
    public Action OnPlayerImmunityEnd;
    public Action OnPlayerDeath;
    public Action OnPlayerHit;
    #endregion

    /// <summary>
    /// Riferimento alla grafica
    /// </summary>
    PlayerGraphicController playerGraphic;
    /// <summary>
    /// Riferimento alla grafica attiva
    /// </summary>
    IGraphic _activeGraphic;
    IGraphic activeGraphic
    {
        set
        {
            _activeGraphic = value;
            shootCtrl.SetAimObject(_activeGraphic.GetAimObject());
        }
        get
        {
            return _activeGraphic;
        }
    }
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
    /// <summary>
    /// Referenza al VFX controller
    /// </summary>
    private PlayerVFXController vfxCtrl;

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

        vfxCtrl = GetComponentInChildren<PlayerVFXController>();
        if (vfxCtrl != null)
            vfxCtrl.Init(this);

        //Setup cose locali
        playerGraphic = GetComponentInChildren<PlayerGraphicController>();
        if (playerGraphic != null)
        {
            activeGraphic = playerGraphic;
            playerGraphic.OnModelChanged += HandleOnPlayerModeloChanged;
            playerGraphic.Init();
        }        
    }

    #region Parasite State
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
        shootCtrl.SetCanAim(false);
        shootCtrl.ChangeShotType(shootCtrl.GetShotSettingByBullet(_e.GetBulletType()));
        _e.Parasite(this);
        collisionCtrl.CheckEnemyCollision(false);
        collisionCtrl.CheckDamageableCollision(false);

        #region Animazione(per ora fatta a caso)
        Vector3 enemyPosition = _e.gameObject.transform.position;
        enemyPosition.z = transform.position.z;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOJump(enemyPosition, 1, 1, 0.5f)).Insert(0, activeGraphic.GetModel().transform.DOScale(0.5f, 0.5f));
        yield return sequence.WaitForCompletion();
        #endregion

        ChangeGraphics(_e.GetGraphics());
        animCtrl.SetAnimator(_e.GetAnimationController().GetAnimator());

        collisionCtrl.CheckEnemyCollision(true);
        collisionCtrl.CheckDamageableCollision(true);
        shootCtrl.SetCanShoot(true);
        shootCtrl.SetCanAim(true);
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
        shootCtrl.SetCanAim(false);
        collisionCtrl.CheckEnemyCollision(false);
        collisionCtrl.CheckDamageableCollision(false);

        #region Animazione(per ora fatta a caso)
        Vector3 platformPosition = _e.gameObject.transform.position;
        platformPosition.z = transform.position.z;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOJump(platformPosition, 1, 1, 0.5f)).Insert(0, activeGraphic.GetModel().transform.DOScale(0.5f, 0.5f));
        yield return sequence.WaitForCompletion();
        #endregion

        collisionCtrl.CheckEnemyCollision(true);
        collisionCtrl.CheckDamageableCollision(true);
        ChangeGraphics(_e.GetGraphics());
        if (playerSM.OnPlayerPlatformParaiste != null)
            playerSM.OnPlayerPlatformParaiste(_e);
    }
    #endregion

    #region Normal State
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
        collisionCtrl.CheckEnemyCollision(false);
        collisionCtrl.CheckDamageableCollision(false);
        shootCtrl.SetCanAim(false);
        shootCtrl.SetCanShoot(false);
        ChangeGraphics(playerGraphic);
        animCtrl.SetAnimator(animCtrl.GetPlayerAnimator());
        shootCtrl.ChangeShotType(shootCtrl.GetPlayerDefaultShotSetting());
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
        activeGraphic.GetModel().transform.DOScale(1, 0.5f);
        yield return null;
        #endregion
        shootCtrl.SetCanAim(true);
        shootCtrl.SetCanShoot(true);
        collisionCtrl.CheckEnemyCollision(true);
        collisionCtrl.CheckDamageableCollision(true);
        if (playerSM.OnPlayerNormal != null)
            playerSM.OnPlayerNormal();
    }
    #endregion

    #region Immunity
    bool immunity;
    /// <summary>
    /// Funzione che attiva la coroutine ImmunityCoroutine
    /// </summary>
    /// <param name="_immunityDuration"></param>
    public void StartImmunityCoroutine(float _immunityDuration)
    {
        immunity = true;
        StartCoroutine(ImmunityCoroutine(_immunityDuration));
    }

    /// <summary>
    /// Funzione che stoppa la coroutine ImmunityCoroutine
    /// </summary>
    public void StopImmunityCoroutine()
    {
        immunity = false;
    }

    /// <summary>
    /// Coroutine che rende il player immune e avvisa quando è finità l'immunità
    /// </summary>
    /// <param name="_immunityDuration"></param>
    /// <returns></returns>
    private IEnumerator ImmunityCoroutine(float _immunityDuration)
    {
        GetCollisionController().CheckEnemyCollision(false);
        gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");
        float timer = _immunityDuration;
        while (immunity && timer > 0)
        {
            activeGraphic.Disable();
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
            activeGraphic.Enable();
            yield return new WaitForSeconds(0.2f);
            timer -= 0.2f;
        }

        GetCollisionController().CheckEnemyCollision(true);
        gameObject.layer = LayerMask.NameToLayer("Player");
        if (OnPlayerImmunityEnd != null)
            OnPlayerImmunityEnd();
    }
    #endregion

    #region Death State
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
    #endregion

    #region Graphic
    /// <summary>
    /// Funzione che cambia la grafica con quella passata come parametro
    /// </summary>
    /// <param name="_newGraphic"></param>
    public void ChangeGraphics(IGraphic _newGraphic)
    {
        EnableGraphics(false);
        activeGraphic = _newGraphic;
        EnableGraphics(true);
    }

    /// <summary>
    /// Funzione che attiva/disattiva la grafica in base al parametro passato
    /// </summary>
    /// <param name="_switch"></param>
    public void EnableGraphics(bool _switch)
    {
        if (_switch)
            activeGraphic.Enable();
        else
            activeGraphic.Disable();
    }

    /// <summary>
    /// Funzione che gestisce l'evento playerGraphic.OnModelChanged
    /// </summary>
    public void HandleOnPlayerModeloChanged()
    {
        if (activeGraphic == (playerGraphic as IGraphic))
            shootCtrl.SetAimObject(activeGraphic.GetAimObject());
    }
    #endregion

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
    /// <summary>
    /// Funzione che ritorna il lives controller
    /// </summary>
    /// <returns></returns>
    public PlayerLivesController GetLivesController()
    {
        return livesCtrl;
    }
    /// <summary>
    /// Funzione che ritorna l'animator controller
    /// </summary>
    /// <returns></returns>
    public PlayerAnimationController GetAnimatorController()
    {
        return animCtrl;
    }
    /// <summary>
    /// Funzione che ritorna la grafica attiva del player
    /// </summary>
    /// <returns></returns>
    public IGraphic GetActualGraphic()
    {
        return activeGraphic;
    }
    /// <summary>
    /// Funzione che ritorna la grafica del player
    /// </summary>
    /// <returns></returns>
    public IGraphic GetPlayerGraphic()
    {
        return playerGraphic;
    }
    #endregion
    #endregion

    private void OnDisable()
    {
        playerGraphic.OnModelChanged -= HandleOnPlayerModeloChanged;
    }
}
