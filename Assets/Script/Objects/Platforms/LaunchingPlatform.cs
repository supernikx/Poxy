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
    private float respawnTime;
    private bool isActive;
    private ControllableType controllableType = ControllableType.Platform;
    private Collider platfromColldier;
    private EnemyToleranceController toleranceCtrl;
    private Player player = null;
    private Rotation rotationBehaviour;

    [Header("Graphics Settings")]
    [SerializeField]
    private EnemyCommandsSpriteController idleCommandsCtrl;
    [SerializeField]
    private EnemyCommandsSpriteController parasiteCommandCtrl;
    [SerializeField]
    private ParticleSystem ejectParticle;

    [Header("Camera Settings")]
    [SerializeField]
    private Transform objectToFollow;

    private MeshRenderer meshRenderer;

    private Vector3 launchDirection;
    private float prevRotation;

    private Coroutine tickCoroutine;

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
        rotationBehaviour = GetComponent<Rotation>();

        graphics = GetComponentInChildren<IGraphic>();
        if (toleranceCtrl != null)
            toleranceCtrl.Init();

        SetObjectState(true);
        graphics.ChangeTexture(TextureType.Default);
        Parasite += HandleParasite;
        LevelManager.OnPlayerDeath += HandleOnPlayerDeath;


        idleCommandsCtrl.Init();
        parasiteCommandCtrl.Init();

        idleCommandsCtrl.ToggleButton(false);
        parasiteCommandCtrl.ToggleButton(false);
        
        prevRotation = 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, prevRotation));
    }

    public void EndParasite()
    {
        player.OnPlayerMaxHealth -= HandlePlayerMaxHealth;
        player = null;
        ejectParticle.Play();
        if (tickCoroutine != null)
        {
            StopCoroutine(tickCoroutine);
            tickCoroutine = null;
        }

        toleranceCtrl.SetActive(false);
        
        parasiteCommandCtrl.ToggleButton(false);
        idleCommandsCtrl.ToggleButton(false);

        SetObjectState(false);
        StartCoroutine(Respawn());

        PlatformManager.OnParasiteEnd(this);
    }

    public void RotationUpdate(Vector2 _aimVector)
    {
        if (_aimVector != Vector2.zero)
        {
            float rotationZ = Mathf.Atan2(_aimVector.y, _aimVector.x) * Mathf.Rad2Deg;
            if (prevRotation != rotationZ && (Mathf.Abs(_aimVector.x) >= 0.5 || Mathf.Abs(_aimVector.y) >= 0.5))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationZ));
                parasiteCommandCtrl.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                idleCommandsCtrl.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                prevRotation = rotationZ;
                launchDirection = _aimVector.normalized;
            }
        }
    }
    #endregion

    #region Handlers
    private void HandleParasite(Player _player)
    {
        player = _player;
        PlatformManager.OnParasite(this);

        toleranceCtrl.Setup();
        player.OnPlayerMaxHealth += HandlePlayerMaxHealth;

        graphics.ChangeTexture(TextureType.Parasite);

        rotationBehaviour.enabled = false;
        launchDirection = transform.right;

        idleCommandsCtrl.ToggleButton(false);
        parasiteCommandCtrl.ToggleButton(true);

        tickCoroutine = StartCoroutine(Tick());
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
        prevRotation = 90f;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, prevRotation));
        parasiteCommandCtrl.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        idleCommandsCtrl.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        graphics.ChangeTexture(TextureType.Default);
        SetObjectState(true);

        parasiteCommandCtrl.ToggleButton(false);
        idleCommandsCtrl.ToggleButton(false);

        rotationBehaviour.enabled = true;
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

    public Vector3 GetLaunchDirection()
    {
        return launchDirection;
    }

    public Transform GetObjectToFollow()
    {
        return objectToFollow;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            if (!parasiteCommandCtrl.IsActive())
                idleCommandsCtrl.ToggleButton(true);
        }
    }

    private void OnDisable()
    {
        if (player != null)
            player.OnPlayerMaxHealth -= HandlePlayerMaxHealth;
        Parasite -= HandleParasite;
        LevelManager.OnPlayerDeath -= HandleOnPlayerDeath;
    }
}
