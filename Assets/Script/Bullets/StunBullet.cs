using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe del proiettile stun
/// </summary>
public class StunBullet : BulletBase
{
    protected override bool OnBulletCollision(Collision _collision)
    {
        if (ownerObject.tag == "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IEnemy enemyHit = _collision.gameObject.GetComponent<IEnemy>();
            if (enemyHit != null)
            {
                enemyHit.StunHit();
                enemyHit.ApplyKnockback(shotDirection.Value, enemyKnockbackForce);
                EnemyBase enemyBase = (enemyHit as EnemyBase);
                if (enemyBase != null && enemyBase.OnEnemyHit != null)
                    enemyBase.OnEnemyHit();
            }
        }
        else if (ownerObject.tag == "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            IButton _target = _collision.gameObject.GetComponent<IButton>();
            if (_target.GetTriggerType() == ButtonTriggerType.Shot)
                _target.Activate();
            else
                return false;
        }

        return base.OnBulletCollision(_collision);
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
}
