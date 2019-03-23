using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerShotController : MonoBehaviour
{
    #region Delegates
    public delegate void ShotDelegate();
    public static ShotDelegate OnShot;
    public delegate void ShotBulletDelegate(ObjectTypes _bullet);
    public static ShotBulletDelegate OnEnemyBulletChanged;
    #endregion

    [Header("Shoot Settings")]
    [SerializeField]
    private Transform shotPoint;
    private Transform shotPointInUse;
    [SerializeField]
    private GameObject aimObject;
    [SerializeField]
    private GameObject crossAir;
    [SerializeField]
    private float crossAirDistance;
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
    /// Boolean che definisce se il player può sparare
    /// </summary>
    bool canShot;
    /// <summary>
    /// Boolean che definisce se il player può mirare
    /// </summary>
    bool canAim;

    void Update()
    {
        //Controllo se posso mirare
        if (canAim)
        {
            //Miro nella direzione in cui mi sto muovendo
            Aim(PlayerInputManager.GetMovementVector());

            //Controllo se posso sparare e se sto premendo il tasto
            if (canShot && PlayerInputManager.IsShooting())
            {
                //Controllo se il firing rate è finito
                if (CheckFiringRate())
                {
                    //Sparo
                    if (OnShot != null)
                        OnShot();
                }
            }
        }

        //Aumento il contatore del firing rate
        firingRateTimer -= Time.deltaTime;
    }

    #region Aim
    /// <summary>
    /// Variabile che indica la direzione in cui si deve sparare
    /// </summary>
    Vector2 direction;

    /// <summary>
    /// Funzione che muove l'aimobject e il personaggio che si sta controllando nella direzione in cui ci si sta muovendo
    /// </summary>
    /// <param name="_movementDirection"></param>
    private void Aim(Vector2 _movementDirection)
    {
        //Calcolo la rotazione del modello in base alla direzione in cui sta andando
        if (_movementDirection.x != 0)
            player.GetActualGraphic().gameObject.transform.rotation = (_movementDirection.x == 1) ? Quaternion.Euler(Vector3.zero) : Quaternion.Euler(0.0f, 180.0f, 0.0f);

        //Calcolo la rotazione dell'aim object
        Vector3 rotationVector = new Vector3(90f, 0f, 90f);
        if (_movementDirection.x == 0 && _movementDirection.y == 1)
            rotationVector.z = 180f;
        else if (_movementDirection.x != 0 && _movementDirection.y == 1)
            rotationVector.z = 135f;
        else if (_movementDirection.x != 0 && _movementDirection.y == 0)
            rotationVector.z = 90f;
        else if (_movementDirection.y == -1)
            rotationVector.z = 45f;

        //Applico la rotazione all'aim object
        aimObject.transform.localEulerAngles = rotationVector;

        //Posiziono il mirino nel punto in cui si sta mirando
        crossAir.transform.position = transform.position + aimObject.transform.right * crossAirDistance;

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
    /// Funzione che prende il proiettile attivo dal pool manager e lo imposta per sparare
    /// </summary>
    private void ShotActiveBullet()
    {
        IBullet bullet = pool.GetPooledObject(shotSettingsInUse.bulletType, gameObject).GetComponent<IBullet>();
        if (bullet != null)
        {
            bullet.Shot(shotSettingsInUse.damage, shotSettingsInUse.shotSpeed, shotSettingsInUse.range, shotPointInUse.position, direction);
        }
    }
    #endregion

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init(Player _player, PoolManager _poolManager)
    {
        player = _player;
        pool = _poolManager;
        ChangeShotType(stunShotSettings);
        SetShotPoint(GetShotPoint());
        OnShot += ShotActiveBullet;
        canAim = true;
        canShot = true;
    }

    #region Setter
    /// <summary>
    /// Funzione che imposta se il player può sparare o no
    /// </summary>
    /// <param name="_newValue"></param>
    public void SetCanShoot(bool _newValue)
    {
        canShot = _newValue;
    }

    /// <summary>
    /// Funzione che imposta se il player può mirare
    /// </summary>
    /// <param name="_newValue"></param>
    public void SetCanAim(bool _newValue)
    {
        canAim = _newValue;
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
    /// Funzione che imposta l'aim object con il parametro passato
    /// </summary>
    /// <param name="_aimObject"></param>
    public void SetAimObject(GameObject _aimObject)
    {
        if (_aimObject != null)
            aimObject = _aimObject;
    }

    /// <summary>
    /// Funzone che cambia il tipo di sparo
    /// </summary>
    public void ChangeShotType(ShotSettings _shotSettings)
    {
        shotSettingsInUse = _shotSettings;
        if (OnEnemyBulletChanged != null)
            OnEnemyBulletChanged(_shotSettings.bulletType);
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
    /// Funzione che ritorna i shot settings di default del player
    /// </summary>
    /// <returns></returns>
    public ShotSettings GetPlayerDefaultShotSetting()
    {
        return stunShotSettings;
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

    private void OnDisable()
    {
        OnShot -= ShotActiveBullet;
    }
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
