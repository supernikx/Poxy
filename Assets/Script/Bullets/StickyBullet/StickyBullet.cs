using UnityEngine;
using System.Collections;

public class StickyBullet : BulletBase
{
    [Header("Sticky Bullet Settings")]
    [SerializeField]
    private GameObject stickyObjectPrefab;
    [SerializeField]
    private GameObject rightDebug;
    [SerializeField]
    private GameObject leftDebug;

    /// <summary>
    /// Offset del bound del collider
    /// </summary>
    private float collisionOffset = 0.015f;
    [SerializeField]
    /// <summary>
    /// Numero di raycast per lo sticky object
    /// </summary>
    private int stickyObjectRayCount = 4;
    private float stickyObjectRaySpacing;
    [SerializeField]
    /// <summary>
    /// Layer per le collisioni degli oggetti
    /// </summary>
    private LayerMask stickyObjectCollisionLayer;

    public override void Setup()
    {
        base.Setup();
        GameObject stickyObject = PoolManager.instance.GetPooledObject(ObjectTypes.SticyObject, gameObject);
        CalculateRaySpacing(stickyObject.GetComponent<BoxCollider>());
    }

    protected override void Move()
    {
        if (CurrentState == State.InUse)
        {
            Vector3 _movementDirection = transform.right * speed;
            if (!Checkcollisions(_movementDirection))
            {
                transform.position += _movementDirection * Time.deltaTime;
                if (Vector3.Distance(shotPosition.position, transform.position) >= range)
                {
                    ObjectDestroyEvent();
                }
            }
        }
    }

    protected override void OnBulletCollision(RaycastHit _collisionInfo)
    {
        if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _collisionInfo.transform.gameObject.GetComponent<IEnemy>().DamageHit(this);
        }

        if (ownerObject.tag != "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = _collisionInfo.transform.gameObject.GetComponent<Player>();
            if (player != null)
                player.GetHealthController().LoseHealth(damage);
            else
                _collisionInfo.transform.gameObject.GetComponent<IEnemy>().GetToleranceCtrl().AddTollerance(damage);
        }

        if (_collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            SpawnStickyObject(_collisionInfo.point, _collisionInfo.normal);
            ObjectDestroyEvent();
        }

    }

    private void SpawnStickyObject(Vector3 _spawnPosition, Vector3 _normal)
    {
        StickyObject stickyObject = PoolManager.instance.GetPooledObject(ObjectTypes.SticyObject, gameObject).GetComponent<StickyObject>();
        stickyObject.Init(_spawnPosition, Quaternion.LookRotation(_normal, Vector3.forward));

        Vector3 leftMaxSize = VerticalCollisions(stickyObject, _normal, 1);
        Vector3 rightMaxSize = VerticalCollisions(stickyObject, _normal, -1);
        Instantiate(rightDebug, rightMaxSize, Quaternion.identity);
        Instantiate(leftDebug, leftMaxSize, Quaternion.identity);
        stickyObject.Spawn(rightMaxSize, leftMaxSize);
    }

    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast sia verticali che orrizontali
    /// </summary>
    private void CalculateRaySpacing(BoxCollider _boxCollider)
    {
        stickyObjectRayCount = Mathf.Clamp(stickyObjectRayCount, 2, int.MaxValue);
        stickyObjectRaySpacing = _boxCollider.bounds.size.x * 0.5f / (stickyObjectRayCount - 1);
    }

    private Vector3 VerticalCollisions(StickyObject _stickyObject, Vector3 _normal, int _direction)
    {
        BoxCollider _collider = _stickyObject.GetBoxCollider();

        //Determina la lunghezza del raycast
        float rayLenght = collisionOffset;

        Vector3 previewRayOrigin = _collider.bounds.center;

        //Cicla tutti i punti da cui deve partire un raycast sull'asse verticale
        for (int i = 0; i < stickyObjectRayCount; i++)
        {
            //Determina il punto da cui deve partire il ray
            Vector3 rayOrigin = _collider.bounds.center;
            rayOrigin += _stickyObject.transform.right * _direction * (stickyObjectRaySpacing * i);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, -_normal);
            RaycastHit hit;
            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght, stickyObjectCollisionLayer))
            {
                rayLenght = hit.distance;
                previewRayOrigin = rayOrigin;
            }
            else
            {
                break;
            }
        }

        return previewRayOrigin;
    }
}
