using UnityEngine;
using System.Collections;
using System;

public class LaunchingPlatform : Platform, IControllable
{
    #region Delegates
    public delegate void ParasiteEvent(Player _player);
    public ParasiteEvent Parasite;
    #endregion

    [Header("Platform Specific Settings")]
    [SerializeField]
    private GameObject graphics;
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

    private void SetObjectState(bool _state)
    {
        isActive = _state;
        platfromColldier.enabled = _state;
        graphics.SetActive(_state);
    }

    #region API
    public override void Init()
    {
        platfromColldier = GetComponent<BoxCollider>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        toleranceCtrl = GetComponent<EnemyToleranceController>();
        if (toleranceCtrl != null)
            toleranceCtrl.Init();

        SetObjectState(true);
        meshRenderer.material = defaultMaterial;
        Parasite += HandleParasite;
        LevelManager.OnPlayerDeath += HandleOnPlayerDeath;
    }

    public void EndParasite()
    {
        player.OnPlayerMaxHealth -= HandlePlayerMaxHealth;
        player = null;

        StopCoroutine(Tick());

        toleranceCtrl.SetActive(false);

        SetObjectState(false);
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

        SetObjectState(true);
    }
    #endregion

    #region Getters
    public ControllableType GetControllableType()
    {
        return controllableType;
    }

    public GameObject GetGraphics()
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
