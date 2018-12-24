using UnityEngine;
using System.Collections;

public class DamageBullet : BulletBase
{
    protected override void OnBulletCollision(RaycastHit _collisionInfo)
    {
        Debug.Log("Hit" + _collisionInfo.transform.gameObject.name);

        if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (EnemyManager.OnEnemyDeath != null)
                EnemyManager.OnEnemyDeath(_collisionInfo.transform.gameObject.GetComponent<IEnemy>());
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
