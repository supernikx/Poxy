using UnityEngine;
using System.Collections;

public class DamageBullet : BulletBase
{
    protected override void OnBulletCollision(RaycastHit _collisionInfo)
    {
        if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _collisionInfo.transform.gameObject.GetComponent<IEnemy>().DamageHit(this);
        }

        if (ownerObject.tag != "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _collisionInfo.transform.gameObject.GetComponent<Player>().GetHealthController().LoseHealth(damage);
        }

        if (_collisionInfo.transform.gameObject != ownerObject)
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
