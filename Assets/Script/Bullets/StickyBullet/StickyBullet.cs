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
    private int percentageLife;
    [SerializeField]
    private int timeInSeconds;

    protected override void Move()
    {
        Vector3 _movementDirection = transform.right * speed;
        transform.position += _movementDirection * Time.deltaTime;
        if (Vector3.Distance(shotPosition, transform.position) >= range)
        {
            ObjectDestroyEvent();
        }
    }

    protected override bool OnBulletCollision(Collision _collision)
    {
        if (ownerObject != null && ownerObject.tag == "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IEnemy enemyHit = _collision.gameObject.GetComponent<IEnemy>();
            if (enemyHit != null)
            {
                enemyHit.DamageHit(GetBulletDamage());
                EnemyBase enemyBase = (enemyHit as EnemyBase);
                if (enemyBase != null && enemyBase.OnEnemyHit != null)
                    enemyBase.OnEnemyHit();
            }
        }

        else if (ownerObject != null && ownerObject.tag != "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = _collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                float damage = player.GetHealthController().GetHealth() * percentageLife / 100;
                player.GetHealthController().DamageHit(damage, timeInSeconds);
            }
            else
            {
                IEnemy enemyHit = _collision.gameObject.GetComponent<IEnemy>();
                if (enemyHit != null)
                {
                    player = enemyHit.gameObject.GetComponentInParent<Player>();
                    float damage = player.GetHealthController().GetHealth() * percentageLife / 100;
                    enemyHit.GetToleranceCtrl().AddTolerance(damage, timeInSeconds);
                }
            }

            if (player.OnPlayerHit != null)
                player.OnPlayerHit();
        }

        else if (ownerObject != null && ownerObject.tag == "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            IButton _target = _collision.gameObject.GetComponent<IButton>();
            if (_target.GetTriggerType() == ButtonTriggerType.Shot)
                _target.Activate();
            else
                return false;
        }

        else if (_collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            ContactPoint contact = _collision.contacts[0];
            Vector3 collisionPoint = contact.point;
            collisionPoint.z = 0f;
            Vector3 closePoint = _collision.collider.ClosestPointOnBounds(collisionPoint);
            SpawnStickyObject(closePoint, contact.normal);
        }

        return base.OnBulletCollision(_collision);
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
        bulletParticle.Stop();
        base.ObjectDestroyEvent();
    }

    protected override void ObjectSpawnEvent()
    {
        muzzleFlashParticle.transform.position = shotPosition;
        muzzleFlashParticle.transform.eulerAngles = new Vector3(0, 0, shotAngle);
        muzzleFlashParticle.Play();
        bulletParticle.Play();
        base.ObjectSpawnEvent();
    }
    #endregion
}
