using UnityEngine;
using System.Collections;

public class StickyBullet : BulletBase
{
    [Header("Sticky Bullet Settings")]
    [SerializeField]
    private ObjectTypes stickyObjectType;

    public override void Setup()
    {
        base.Setup();
        GameObject stickyObject = PoolManager.instance.GetPooledObject(ObjectTypes.StickyObject, gameObject);
        CalculateRaySpacing(stickyObject.GetComponent<StickyObject>());
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

    /// <summary>
    /// Funzione che spawna uno sticky object 
    /// </summary>
    /// <param name="_spawnPosition"></param>
    /// <param name="_normal"></param>
    private void SpawnStickyObject(Vector3 _spawnPosition, Vector3 _normal)
    {
        StickyObject stickyObject = PoolManager.instance.GetPooledObject(stickyObjectType, gameObject).GetComponent<StickyObject>();
        stickyObject.Init(_spawnPosition, Quaternion.LookRotation(_normal, Vector3.forward));

        Vector3 leftMaxSize = CheckSpace(stickyObject, _normal, 1);
        Vector3 rightMaxSize = CheckSpace(stickyObject, _normal, -1);
        stickyObject.Spawn(rightMaxSize, leftMaxSize);
    }

    #region CheckSpace
    /// <summary>
    /// Funzione che calcola lo spazio tra i raycast sia verticali che orrizontali
    /// </summary>
    private float stickyObjectRaySpacing;
    private void CalculateRaySpacing(StickyObject _stickyObject)
    {        
        stickyObjectRaySpacing = _stickyObject.GetBoxCollider().bounds.size.x * 0.5f / (_stickyObject.GetRayCount() - 1);
    }

    /// <summary>
    /// Funzione che calcola ritorna l'ultimo punto con cui si collide con il collision layer
    /// nella direzione passata come parametro
    /// </summary>
    /// <param name="_stickyObject"></param>
    /// <param name="_normal"></param>
    /// <param name="_direction"></param>
    /// <returns></returns>
    private Vector3 CheckSpace(StickyObject _stickyObject, Vector3 _normal, int _direction)
    {
        BoxCollider _collider = _stickyObject.GetBoxCollider();

        float rayLenght = 1f;

        Vector3 previewRayOrigin = _collider.bounds.center;

        for (int i = 0; i < _stickyObject.GetRayCount(); i++)
        {
            //Determina il punto da cui deve partire il ray
            Vector3 rayOrigin = _collider.bounds.center;
            rayOrigin += _stickyObject.transform.right * _direction * (stickyObjectRaySpacing * i);

            //Crea il ray
            Ray ray = new Ray(rayOrigin, -_normal);
            RaycastHit hit;

            //Eseguo il raycast
            if (Physics.Raycast(ray, out hit, rayLenght, _stickyObject.GetCollisionLayer()))
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
    #endregion
}
