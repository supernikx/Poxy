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
    [SerializeField]
    private LayerMask layersToCheck;

    private BulletSoundController soundCtrl;

    #region Sticky
    StickyObject stickyObject;
    private bool spawnSticky;
    #endregion

    public override void Setup()
    {
        base.Setup();
        soundCtrl = GetComponentInChildren<BulletSoundController>();
        spawnSticky = false;
        stickyObject = null;
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

        return base.OnBulletCollision(_collider);
    }

    #region Spawn/Destroy
    protected override void ObjectDestroyEvent()
    {
        soundCtrl.Hit();
        bulletParticle.Stop();

        if (spawnSticky)
            stickyObject.Spawn();
        base.ObjectDestroyEvent();
    }

    protected override void ObjectSpawnEvent()
    {
        muzzleFlashParticle.transform.position = shotPosition;
        muzzleFlashParticle.transform.eulerAngles = new Vector3(0, 0, shotAngle);
        muzzleFlashParticle.Play();
        soundCtrl.Shot();
        bulletParticle.Play();

        Ray ray = new Ray(transform.position, transform.right);
        RaycastHit hitInfo;
        if (Physics.SphereCast(ray, bulletCollider.radius ,out hitInfo, 100f, layersToCheck))
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
            {
                spawnSticky = true;
                stickyObject = PoolManager.instance.GetPooledObject(stickyObjectType, gameObject).GetComponent<StickyObject>();
                stickyObject.Init(hitInfo.point, hitInfo.normal, Quaternion.LookRotation(Vector3.forward, hitInfo.normal));
            }
            else
            {
                spawnSticky = false;
                stickyObject = null;
            }
        }
        else
        {
            spawnSticky = false;
            stickyObject = null;
        }

        base.ObjectSpawnEvent();
    }
    #endregion
}
