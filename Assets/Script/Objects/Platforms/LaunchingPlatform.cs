using UnityEngine;
using System.Collections;
using System;

public class LaunchingPlatform : PlatformBase, IControllable
{
    #region Delegates
    public delegate void ParasiteEvent(Player _player);
    public ParasiteEvent Parasite;
    #endregion

    [Header("Platform Specific Settings")]
    [SerializeField]
    private IGraphic graphics;
    [SerializeField]
    private int respawnTime;
    private bool isActive;
    private ControllableType controllableType = ControllableType.Platform;
    private Collider platfromColldier;
    private EnemyToleranceController toleranceCtrl;
    private Player player = null;

    [Header("Graphics Settings")]
    [SerializeField]
    private Material defaultMaterial;
    [SerializeField]
    private Material infectedMaterial;

    private MeshRenderer meshRenderer;

    private Vector3 direction;
    private int prevRotation;

    private void SetObjectState(bool _state)
    {
        isActive = _state;
        platfromColldier.enabled = _state;

        if (_state)
            graphics.Enable();
        else
            graphics.Disable();
    }

    #region API
    public override void Init()
    {
        platfromColldier = GetComponent<BoxCollider>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        toleranceCtrl = GetComponent<EnemyToleranceController>();
        graphics = GetComponentInChildren<IGraphic>();
        if (toleranceCtrl != null)
            toleranceCtrl.Init();

        SetObjectState(true);
        meshRenderer.material = defaultMaterial;
        Parasite += HandleParasite;
        LevelManager.OnPlayerDeath += HandleOnPlayerDeath;

        prevRotation = 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, prevRotation));
    }

    public void EndParasite()
    {
        player.OnPlayerMaxHealth -= HandlePlayerMaxHealth;
        player = null;

        StopCoroutine(Tick());

        toleranceCtrl.SetActive(false);

        SetObjectState(false);
        prevRotation = 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, prevRotation));
        StartCoroutine(Respawn());

        meshRenderer.material = defaultMaterial;

        PlatformManager.OnParasiteEnd(this);
    }
    #endregion

    #region Handlers
    private void HandleParasite(Player _player)
    {
        player = _player;
        PlatformManager.OnParasite(this);

        toleranceCtrl.Setup();
        player.OnPlayerMaxHealth += HandlePlayerMaxHealth;

        meshRenderer.material = infectedMaterial;

        StartCoroutine(Tick());
    }

    private void HandlePlayerMaxHealth()
    {
        toleranceCtrl.SetActive(true);
    }

    private void HandleOnPlayerDeath()
    {
        StopCoroutine(Respawn());
        SetObjectState(true);
    }
    #endregion

    #region Coroutines
    // PER QUALCHE MOTIVO QUESTA RIMANE SEMPRE ATTIVA
    private IEnumerator Tick()
    {
        while (true)
        {
            if (toleranceCtrl.IsActive())
            {
                toleranceCtrl.AddTolleranceOvertime();

                if (toleranceCtrl.CheckTolerance())
                {
                    if (toleranceCtrl.OnMaxTolleranceBar != null)
                        toleranceCtrl.OnMaxTolleranceBar();
                }
            }

            Vector3 _input = PlayerInputManager.GetMovementVector();
            
            int _targetRotation = -1;

            if (_input.x == 0)
            {
                if (_input.y > 0)
                    _targetRotation = 90;
                else if (_input.y < 0)
                    _targetRotation = 270;
            }
            else if (_input.y == 0)
            {
                if (_input.x > 0)
                    _targetRotation = 0;
                else if (_input.x < 0)
                    _targetRotation = 180;
            }
            else
            {
                if (_input.x > 0 && _input.y > 0)
                    _targetRotation = 45;
                else if (_input.x < 0 && _input.y > 0)
                    _targetRotation = 135;
                else if (_input.x < 0 && _input.y < 0)
                    _targetRotation = 225;
                else if (_input.x > 0 && _input.y < 0)
                    _targetRotation = 315;
            }

            if (_targetRotation >= 0 && prevRotation != _targetRotation)
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, _targetRotation));
                prevRotation = _targetRotation;
                direction = _input.normalized;
            }

            yield return null;
        }
    }

    private IEnumerator Respawn()
    {
        float timer = 0;

        while (timer < respawnTime)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        SetObjectState(true);
    }
    #endregion

    #region Getters
    public ControllableType GetControllableType()
    {
        return controllableType;
    }

    public IGraphic GetGraphics()
    {
        return graphics;
    }

    public EnemyToleranceController GetToleranceCtrl()
    {
        return toleranceCtrl;
    }

    public bool IsActive()
    {
        return isActive;
    }
    #endregion
}
