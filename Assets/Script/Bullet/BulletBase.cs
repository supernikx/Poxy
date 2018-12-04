using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour, IPoolObject, IBullet
{
    #region IPoolObject
    public GameObject ownerObject
    {
        get
        {
            return _ownerObject;
        }
        set
        {
            _ownerObject = value;
        }
    }
    GameObject _ownerObject;

    public State CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
        }
    }
    State _currentState;

    public event PoolManagerEvets.Events OnObjectSpawn;
    public event PoolManagerEvets.Events OnObjectDestroy;
    #endregion

    /// <summary>
    /// Distanza massima che può percorrere il proiettile
    /// </summary>
    protected float range;
    /// <summary>
    /// Velocità a cui va il proiettile
    /// </summary>
    protected float speed;
    /// <summary>
    /// Direzione in cui va il proiettile
    /// </summary>
    protected Vector3 direction;
    /// <summary>
    /// Posizione da cui parte lo sparo
    /// </summary>
    protected Transform shootPosition;

    /// <summary>
    /// Funzione di Setup
    /// </summary>
    public void Setup()
    {
        collider = GetComponent<Collider>();
        CalculateRaySpacing();
    }

    /// <summary>
    /// Funzione che inizializza il proiettile e lo fa sparare
    /// </summary>
    /// <param name="_speed"></param>
    /// <param name="_range"></param>
    /// <param name="_shootPosition"></param>
    /// <param name="_direction"></param>
    public virtual void Shoot(float _speed, float _range, Transform _shootPosition, Vector3 _direction)
    {
        speed = _speed;
        range = _range;
        direction = _direction;
        shootPosition = _shootPosition;
        transform.position = shootPosition.position;

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotationZ);
        ObjectSpawnEvent();
    }

    /// <summary>
    /// Funzione che richiama l'evento di spawn del proiettile
    /// </summary>
    protected void ObjectSpawnEvent()
    {
        if (OnObjectSpawn != null)
            OnObjectSpawn(this);
    }
    /// <summary>
    /// Funzione che richiama l'evento di Destroy del proiettile
    /// </summary>
    protected void ObjectDestroyEvent()
    {
        if (OnObjectDestroy != null)
            OnObjectDestroy(this);
    }

    #region Collision
    [Header("Collision Settings")]
    Collider collider;
    const float colliderOffset = 0.015f;
    [SerializeField]
    int rayCount;
    [SerializeField]
    float rayLenght;
    float raySpacing;
    float test;

    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast
    /// </summary>
    private void CalculateRaySpacing()
    {
        Bounds bulletBound = collider.bounds;
        bulletBound.Expand(colliderOffset * -2f);
        raySpacing = bulletBound.size.y / (rayCount - 1);
        test = bulletBound.size.x / 2;
    }

    /// <summary>
    /// Funzione che controlla se avviene una collisione sugli assi verticali
    /// </summary>
    /// <param name="_movementVelocity"></param>
    protected bool Checkcollisions(Vector3 _direction)
    {
        //Cicla tutti i punti da cui deve partire un raycast
        for (int i = 0; i < rayCount; i++)
        {
            //Determina il punto da cui deve partire il ray (centro del proiettile)
            Vector3 rayOrigin = transform.position - transform.up * (raySpacing * ((rayCount - 1) / 2)) + (transform.right * test);
            rayOrigin += transform.up * (raySpacing * i);
            //Debug.Log(transform.right);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, transform.right);
            RaycastHit hit;

            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght))
            {
                //Se colpisce qualcosa chiama la funzione e ritorna true
                OnBulletCollision(hit);
                return true;
            }

            Debug.DrawRay(rayOrigin, transform.right * rayLenght, Color.red);
        }
        return false;
    }

    /// <summary>
    /// Funzione chiamata quando il proiettile entra in collisione con qualcosa
    /// </summary>
    /// <param name="_collisionInfo"></param>
    protected abstract void OnBulletCollision(RaycastHit _collisionInfo);
    #endregion
}
