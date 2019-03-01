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

    private EnemyToleranceController toleranceCtrl;
    private Player player = null;

    private void SetObjectState (bool _state)
    {
        isActive = _state;
        GetComponent<BoxCollider>().enabled = _state;
        graphics.SetActive(_state);
    }

    #region API
    public override void Init()
    {
        toleranceCtrl = GetComponent<EnemyToleranceController>();
        if (toleranceCtrl != null)
            toleranceCtrl.Init();

        SetObjectState(true);
        GetComponentInChildren<MeshRenderer>().material.color = Color.red;

        Parasite += HandleParasite;
    }

    public void EndParasite()
    {
        player.OnPlayerMaxHealth -= HandlePlayerMaxHealth;
        player = null;

        StopCoroutine(Tick());

        toleranceCtrl.SetActive(false);

        SetObjectState(false);
        StartCoroutine(Respawn());

        PlatformManager.OnParasiteEnd(this);
    }
    #endregion

    #region Handlers
    private void HandleParasite(Player _player)
    {
        player = _player;
        PlatformManager.OnParasite(this);

        GetComponentInChildren<MeshRenderer>().material.color = Color.blue;

        toleranceCtrl.Setup();
        player.OnPlayerMaxHealth += HandlePlayerMaxHealth;

        StartCoroutine(Tick());
    } 

    private void HandlePlayerMaxHealth()
    {
        toleranceCtrl.SetActive(true);
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
        GetComponentInChildren<MeshRenderer>().material.color = Color.red;
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
