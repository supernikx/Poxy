using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe del proiettile stun
/// </summary>
public class StunBullet : BulletBase
{
    protected override bool OnBulletCollision(RaycastHit _collisionInfo)
    {
        if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _collisionInfo.transform.gameObject.GetComponent<IEnemy>().StunHit();
        }
        else if (_collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            _collisionInfo.transform.gameObject.GetComponent<IButton>().Activate();
        }

        return base.OnBulletCollision(_collisionInfo);
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
