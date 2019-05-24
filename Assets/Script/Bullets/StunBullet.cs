using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe del proiettile stun
/// </summary>
public class StunBullet : BulletBase
{
    [Header("Stun Bullet Settings")]
    [SerializeField]
    private ParticleSystem bulletParticle;

    protected override bool OnBulletCollision(Collider _collider)
    {
        if (ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IEnemy enemyHit = _collider.gameObject.GetComponent<IEnemy>();
            if (enemyHit != null)
            {
                enemyHit.StunHit();
                enemyHit.ApplyKnockback(shotDirection.Value, enemyKnockbackForce);
                EnemyBase enemyBase = (enemyHit as EnemyBase);
                if (enemyBase != null && enemyBase.OnEnemyHit != null)
                    enemyBase.OnEnemyHit();
            }
        }
        else if (ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            IButton _target = _collider.gameObject.GetComponent<IButton>();
            if (_target.GetTriggerType() == ButtonTriggerType.Shot)
                _target.Activate();
            else
                return false;
        }

        return base.OnBulletCollision(_collider);
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

    #region Spawn/Destroy
    protected override void ObjectDestroyEvent()
    {
        bulletParticle.Stop();
        base.ObjectDestroyEvent();
    }

    protected override void ObjectSpawnEvent()
    {
        bulletParticle.Play();
        base.ObjectSpawnEvent();
    }
    #endregion
}
