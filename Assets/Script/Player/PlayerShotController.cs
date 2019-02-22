﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerShotController : MonoBehaviour
{
    #region Delegates
    public delegate void ShotDelegate();
    public static ShotDelegate OnShot;
    #endregion

    [Header("Shoot Settings")]
    [SerializeField]
    private Transform shotPoint;
    private Transform shotPointInUse;
    [SerializeField]
    private GameObject aimObject;
    [SerializeField]
    ShotSettings stunShotSettings;
    [SerializeField]
    private List<ShotSettings> damageShotSettings = new List<ShotSettings>();
    ShotSettings shotSettingsInUse;

    //private bool canShot;

    /// <summary>
    /// Referenza al player
    /// </summary>
    Player player;
    /// <summary>
    /// Referenza al pool manager
    /// </summary>
    PoolManager pool;
    /// <summary>
    /// Boolean che definisce se posso sparare o no
    /// </summary>
    bool canShotDamage;
    /// <summary>
    /// Boolean che definisce se posso stunnare o no
    /// </summary>
    bool canShot;

    void Update()
    {
        // non so se mettere il bool solo per controllare che possa sparare stun possa rompere qualcosa,
        // eventualmente si cambia venerdì
        if (canShot)
        {
            if (UseMouseInput())
            {
                MouseAim();

                if (Input.GetButton("LeftMouse"))
                {
                    //Controllo se posso sparare
                    if (CheckFiringRate())
                    {
                        if (OnShot != null)
                            OnShot();
                    }
                }
            }
            else
            {
                JoystickAim();

                if (InputManager.GetRT())
                {
                    //Controllo se posso sparare
                    if (CheckFiringRate())
                    {
                        if (OnShot != null)
                            OnShot();
                    }
                }
            }
        }

        //Aumento il contatore del firing rate
        firingRateTimer -= Time.deltaTime;
    }

    #region Aim
    /// <summary>
    /// Funzione che ruota l'arma in base alla posizione del mouse
    /// </summary>
    Vector2 direction;
    private void MouseAim()
    {
        float rotationZ;
        //Converto la posizione da cui devo sparare da world a screen
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(shotPointInUse.position);
        //Calcolo la poszione del mouse
        Vector2 mousePosition = (Input.mousePosition - screenPoint).normalized;
        //Calcolo la rotazione che deve fare il fucile
        rotationZ = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;

        //Ruoto l'arma nella direzione in cui si sta mirando
        if (mousePosition.x < 0)
        {
            //Se si sta mirando nel senso opposto flippo l'arma
            if (rotationZ > -145f && rotationZ <= -90f)
            {
                aimObject.transform.rotation = Quaternion.Euler(Mathf.PI * Mathf.Rad2Deg, 0.0f, 145f);
            }
            else
            {
                aimObject.transform.rotation = Quaternion.Euler(Mathf.PI * Mathf.Rad2Deg, 0.0f, -rotationZ);
            }
            player.GetActualGraphic().transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }
        else
        {
            if (rotationZ >= -90f && rotationZ < -35f)
            {
                aimObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -35f);
            }
            else
            {
                aimObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            }

            player.GetActualGraphic().transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        }

        //Calcolo la direzione di sparo
        direction = aimObject.transform.right;
    }

    /// <summary>
    /// Funzione che muove l'arma in base alla direzione del right stick
    /// </summary>
    private void JoystickAim()
    {
        float rotationZ;
        //Prendo gli input
        Vector2 input = new Vector3(Input.GetAxisRaw("HorizontalJoystickRightStick"), Input.GetAxisRaw("VerticalJoystickRightStick"));
        //Se non muovo lo stick lascio l'arma nella posizione precedente
        if (input.x == 0 && input.y == 0)
            return;

        //Calcolo la rotazione che deve fare il fucile
        rotationZ = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        //Ruoto l'arma nella direzione in cui si sta mirando
        if (input.x < 0)
        {
            //Se si sta mirando nel senso opposto flippo l'arma
            player.GetActualGraphic().transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
            if (rotationZ > -145f && rotationZ <= -90f)
            {
                aimObject.transform.rotation = Quaternion.Euler(Mathf.PI * Mathf.Rad2Deg, 0.0f, 145f);
            }
            else
            {
                aimObject.transform.rotation = Quaternion.Euler(Mathf.PI * Mathf.Rad2Deg, 0.0f, -rotationZ);
            }
        }
        else
        {
            player.GetActualGraphic().transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            if (rotationZ >= -90f && rotationZ < -35f)
            {
                aimObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -35f);
            }
            else
            {
                aimObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
            }
        }

        //Prendo la direzione a cui devo mirare
        direction = aimObject.transform.right;
    }
    #endregion

    #region Shot
    /// <summary>
    /// Funzione che controlla se posso sparare e ritorna true o false
    /// </summary>
    private float firingRateTimer;
    private bool CheckFiringRate()
    {
        if (firingRateTimer < 0)
        {
            firingRateTimer = 1f / shotSettingsInUse.firingRate;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Funzione che prende un proiettile dal pool manager e lo imposta per sparare
    /// </summary>
    private void ShotStunBullet()
    {
        IBullet bullet = pool.GetPooledObject(ObjectTypes.StunBullet, gameObject).GetComponent<IBullet>();
        if (bullet != null)
        {
            bullet.Shot(0, shotSettingsInUse.shotSpeed, shotSettingsInUse.range, shotPointInUse.position, direction);
        }
    }

    /// <summary>
    /// Funzione che prende un proiettile danneggiante dal pool manager e lo imposta per sparare
    /// </summary>
    private void ShotEnemyBullet()
    {
        IBullet bullet = pool.GetPooledObject(enemyShotSetting.bulletType, gameObject).GetComponent<IBullet>();
        if (bullet != null)
        {
            bullet.Shot(shotSettingsInUse.damage, shotSettingsInUse.shotSpeed, shotSettingsInUse.range, shotPointInUse.position, direction);
        }
    }
    #endregion

    /// <summary>
    /// Funzione che controlla se usare gli input del mouse o del controller
    /// </summary>
    /// <returns></returns>
    Vector3 mousePreviewsPos;
    private bool UseMouseInput()
    {
        if (Vector3.Distance(Input.mousePosition, mousePreviewsPos) < 0.1f)
        {
            if (Input.GetButton("LeftMouse"))
                return true;
            if (Input.GetJoystickNames().Where(j => j != "").FirstOrDefault() != null)
                return false;
            return true;
        }

        mousePreviewsPos = Input.mousePosition;
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(shotPoint.position, shotSettingsInUse.range);
    }

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init(Player _player, PoolManager _poolManager)
    {
        player = _player;
        pool = _poolManager;
        canShotDamage = false;
        canShot = true;
        SetShotPoint(GetShotPoint());
    }

    #region Setter
    /// <summary>
    /// Funzione che imposta la variabile can shoot
    /// con quella passata come parametro
    /// </summary>
    /// <param name="_canShoot"></param>
    public void SetCanShootDamage(bool _canShoot)
    {
        canShotDamage = _canShoot;
    }

    /// <summary>
    /// Funzione che imposta se il player può sparare o no
    /// </summary>
    /// <param name="_newValue"></param>
    public void SetCanShoot(bool _newValue)
    {
        canShot = _newValue;
    }

    /// <summary>
    /// Funzione che imposta lo shot point in use con quello passato come parametro
    /// </summary>
    /// <param name="_newShotPoint"></param>
    public void SetShotPoint(Transform _newShotPoint)
    {
        shotPointInUse = _newShotPoint;
    }

    /// <summary>
    /// Funzone che cambia il tipo di sparo
    /// </summary>
    bool useStunBullets;
    public void ChangeShotType()
    {
        //Controllo se posso cambiare tipo di sparo
        if (useStunBullets && canShotDamage)
        {
            OnShot -= ShotStunBullet;
            OnShot += ShotEnemyBullet;
            shotSettingsInUse = enemyShotSetting;
            useStunBullets = false;
        }
        else if (!useStunBullets)
        {
            OnShot -= ShotEnemyBullet;
            OnShot += ShotStunBullet;
            shotSettingsInUse = stunShotSettings;
            useStunBullets = true;
        }
    }

    /// <summary>
    /// Funzione che imposta l'enemyShotSetting con i valori presenti nella lista di shot settings
    /// corrispondenti al tipo di proiettile passato come parametro
    /// </summary>
    ShotSettings enemyShotSetting;
    public void SetEnemyShot(ObjectTypes _enemyBullet)
    {
        ShotSettings newShotSettings = GetShotSettingByBullet(_enemyBullet);
        if (newShotSettings != null)
        {
            enemyShotSetting = newShotSettings;
        }
    }
    #endregion

    #region Getter
    /// <summary>
    /// Funzione che ritorna i Shot Settings in base al tipo di proiettile passato come parametro
    /// </summary>
    /// <param name="_bullet"></param>
    /// <returns></returns>
    public ShotSettings GetShotSettingByBullet(ObjectTypes _bullet)
    {
        foreach (ShotSettings shot in damageShotSettings)
        {
            if (shot.bulletType == _bullet)
                return shot;
        }
        return null;
    }

    /// <summary>
    /// Funzione che ritorna lo shot point del player
    /// </summary>
    /// <returns></returns>
    public Transform GetShotPoint()
    {
        return shotPoint;
    }
    #endregion
    #endregion
}

[System.Serializable]
public class ShotSettings
{
    public ObjectTypes bulletType;
    public int damage;
    public float range;
    public float shotSpeed;
    public float firingRate;
}
