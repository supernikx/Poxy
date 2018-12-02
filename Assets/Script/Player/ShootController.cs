using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootController : MonoBehaviour
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
    bool CanShoot = false;

    /// <summary>
    /// Funzione che inizializza lo script
    /// </summary>
    public void Init()
    {
        pool = PoolManager.instance;
        CanShoot = true;
    }

    void Update()
    {
        if (CanShoot)
        {
            Aim();

            //Controllo se posso sparare
            if (firingRateTimer < 0)
            {
                if (Input.GetButton("Shoot"))
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
        if (UseMouseInput())
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(shootPoint.position);
            direction = (Input.mousePosition - screenPoint).normalized;
            rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }
        else
        {
            Vector2 input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            direction = new Vector3(input.x, input.y);
            rotationZ = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        }
        
        gun.transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
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
}
