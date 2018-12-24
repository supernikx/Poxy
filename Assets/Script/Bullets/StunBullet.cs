using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe del proiettile stun
/// </summary>
public class StunBullet : BulletBase
{
    protected override void OnBulletCollision(RaycastHit _collisionInfo)
    {
        Debug.Log("Hit" + _collisionInfo.transform.gameObject.name);

        if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (EnemyManager.OnEnemyStun != null)
                EnemyManager.OnEnemyStun(_collisionInfo.transform.gameObject.GetComponent<IEnemy>());
        }

        ObjectDestroyEvent();
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
}
