using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerShootController : MonoBehaviour
{
    [Header("Shoot Settings")]
    [SerializeField]
    private Transform shootPoint;
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private float range;
    [SerializeField]
    private float shootSpeed;
    [SerializeField]
    private float firingRate;
    private float firingRateTimer;

    /// <summary>
    /// Referenza al pool manager
    /// </summary>
    PoolManager pool;
    /// <summary>
    /// Boolean che definisce se posso sparare o no
    /// </summary>
    bool canShoot;

    void Update()
    {
        if (canShoot)
        {
            Aim();

            //Controllo se posso sparare
            if (firingRateTimer < 0)
            {
                if (Input.GetAxis("ShootJoystick") > 0 || Input.GetButton("ShootMouse"))
                {
                    Shoot();
                    firingRateTimer = 1f / firingRate;
                }
            }
        }

        //Aumento il contatore del firing rate
        firingRateTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Funzione che ruota l'arma in base alla posizione del mouse o del LeftStick del joystick
    /// </summary>
    Vector2 direction;
    private void Aim()
    {
        float rotationZ;

        //Controllo che input usare
        if (UseMouseInput())
        {
            //Converto la posizione da cui devo sparare da world a screen
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(shootPoint.position);
            //Calcolo la direzione tra la posizione del mouse e lo shoot point
            direction = (Input.mousePosition - screenPoint).normalized;
            //Calcolo la rotazione che deve fare il fucile
            rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        else
        {
            //Prendo gli input
            Vector2 input = new Vector3(Input.GetAxisRaw("HorizontalJoystickRightStick"), Input.GetAxisRaw("VerticalJoystickRightStick"));
            //Se non muovo lo stick lascio l'arma nella posizione precedente
            if (input.x == 0 && input.y == 0)
                return;
            //Prendo la direzione a cui devo mirare
            direction = new Vector3(input.x, input.y);
            //Calcolo la rotazione che deve fare il fucile
            rotationZ = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        }

        //Ruoto l'arma nella direzione in cui si sta mirando
        if (direction.x < 0)
        {
            //Se si sta mirando nel senso opposto flippo l'arma
            gun.transform.rotation = Quaternion.Euler(Mathf.PI * Mathf.Rad2Deg, 0.0f, -rotationZ);
        }
        else
        {
            gun.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        }
    }

    /// <summary>
    /// Funzione che prende un proiettile dal pool manager e lo imposta per sparare
    /// </summary>
    private void Shoot()
    {
        IBullet bullet = pool.GetPooledObject(ObjectTypes.StunBullet, gameObject).GetComponent<IBullet>();
        if (bullet != null)
        {
            bullet.Shoot(shootSpeed, range, shootPoint, direction);
        }
    }

    /// <summary>
    /// Funzione che controlla se usare gli input del mouse o del controller
    /// </summary>
    /// <returns></returns>
    Vector3 mousePreviewsPos;
    private bool UseMouseInput()
    {
        if (Mathf.Approximately(Input.mousePosition.x, mousePreviewsPos.x) && Mathf.Approximately(Input.mousePosition.y, mousePreviewsPos.y))
        {
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
        Gizmos.DrawWireSphere(shootPoint.position, range);
    }

    #region API
    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init(PoolManager _poolManager)
    {
        pool = _poolManager;
        canShoot = false;
    }

    /// <summary>
    /// Funzione che imposta la variabile can shoot
    /// con quella passata come parametro
    /// </summary>
    /// <param name="_canShoot"></param>
    public void SetCanShoot(bool _canShoot)
    {
        canShoot = _canShoot;
    }
    #endregion
}
