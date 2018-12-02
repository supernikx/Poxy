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
        ObjectDestroyEvent();
    }

    private void Update()
    {
        if (CurrentState == State.InUse)
        {
            Vector3 _movementDirection = transform.right * speed;
            if (!Checkcollisions(_movementDirection))
            {
                transform.position += _movementDirection * Time.deltaTime;
                if (Vector3.Distance(shootPosition.position, transform.position) >= range)
                {
                    ObjectDestroyEvent();
                }
            }
        }
    }
}
