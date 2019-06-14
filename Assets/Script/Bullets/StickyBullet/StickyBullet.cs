using UnityEngine;
using System.Collections;

public class StickyBullet : BulletBase
{
    [Header("Sticky Bullet Settings")]
    [SerializeField]
    private ParticleSystem bulletParticle;
    [SerializeField]
    private ParticleSystem muzzleFlashParticle;
    [SerializeField]
    private ObjectTypes stickyObjectType;

    private BulletSoundController soundCtrl;

    public override void Setup()
    {
        base.Setup();
        soundCtrl = GetComponentInChildren<BulletSoundController>();
    }

    protected override void Move()
    {
        Vector3 _movementDirection = transform.right * speed;
        transform.position += _movementDirection * Time.deltaTime;
        if (Vector3.Distance(shotPosition, transform.position) >= range)
        {
            ObjectDestroyEvent();
        }
    }

    protected override bool OnBulletCollision(Collider _collider)
    {
        if (ownerObject != null && ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IEnemy enemyHit = _collider.gameObject.GetComponent<IEnemy>();
            if (enemyHit != null)
            {
                enemyHit.DamageHit(GetBulletDamage());
                EnemyBase enemyBase = (enemyHit as EnemyBase);
                if (enemyBase != null && enemyBase.OnEnemyHit != null)
                    enemyBase.OnEnemyHit();
            }
        }

        else if (ownerObject != null && ownerObject.tag != "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = _collider.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.GetHealthController().DamageHit(GetBulletDamage());
            }
            else
            {
                IEnemy enemyHit = _collider.gameObject.GetComponent<IEnemy>();
                if (enemyHit != null)
                {
                    player = enemyHit.gameObject.GetComponentInParent<Player>();
                    enemyHit.GetToleranceCtrl().AddTolerance(GetBulletDamage());
                }
            }

            if (player.OnPlayerHit != null)
                player.OnPlayerHit();
        }

        else if (ownerObject != null && ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            IButton _target = _collider.gameObject.GetComponent<IButton>();
            if (_target.GetTriggerType() == ButtonTriggerType.Shot)
                _target.Activate();
            else
                return false;
        }

        else if (_collider.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            RaycastHit hit;
            Vector3 endPoint = transform.position + transform.right.normalized;
            if (Physics.Linecast(transform.position, endPoint, out hit))
            {
                Vector3 closePoint = _collider.ClosestPointOnBounds(transform.position);
                SpawnStickyObject(closePoint, hit.normal);
            }
        }

        return base.OnBulletCollision(_collider);
    }

    /// <summary>
    /// Funzione che spawna uno sticky object 
    /// </summary>
    /// <param name="_spawnPosition"></param>
    /// <param name="_normal"></param>
    private void SpawnStickyObject(Vector3 _spawnPosition, Vector3 _normal)
    {
        StickyObject stickyObject = PoolManager.instance.GetPooledObject(stickyObjectType, gameObject).GetComponent<StickyObject>();
        stickyObject.Init(_spawnPosition, Quaternion.LookRotation(Vector3.forward, _normal));
        Vector3 rightMaxPosition = stickyObject.CheckSpace(_normal, 1);
        Vector3 leftMaxPosition = stickyObject.CheckSpace(_normal, -1);
        stickyObject.Spawn(leftMaxPosition, rightMaxPosition);
    }

    #region Spawn/Destroy
    protected override void ObjectDestroyEvent()
    {
        soundCtrl.Hit();
        bulletParticle.Stop();
        base.ObjectDestroyEvent();
    }

    protected override void ObjectSpawnEvent()
    {
        muzzleFlashParticle.transform.position = shotPosition;
        muzzleFlashParticle.transform.eulerAngles = new Vector3(0, 0, shotAngle);
        muzzleFlashParticle.Play();
        soundCtrl.Shot();
        bulletParticle.Play();
        base.ObjectSpawnEvent();
    }
    #endregion
}
