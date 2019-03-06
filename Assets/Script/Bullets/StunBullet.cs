﻿using System.Collections;
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
            IEnemy enemyHit = _collisionInfo.transform.gameObject.GetComponent<IEnemy>();
            if (enemyHit != null)
            {
                enemyHit.StunHit();
                EnemyBase enemyBase = (enemyHit as EnemyBase);
                if (enemyBase != null && enemyBase.OnEnemyHit != null)
                    enemyBase.OnEnemyHit();
            }
        }
        else if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            IButton _target = _collisionInfo.transform.gameObject.GetComponent<IButton>();
            if (_target.GetTriggerType() == ButtonTriggerType.Shot)
                _target.Activate();
            else
                return false;
        }

        return base.OnBulletCollision(_collisionInfo);
    }

    protected override void Move()
    {
        Vector3 _movementDirection = transform.right * speed;
        if (!Checkcollisions(_movementDirection * Time.deltaTime))
        {
            transform.position += _movementDirection * Time.deltaTime;
            if (Vector3.Distance(shotPosition, transform.position) >= range)
            {
                ObjectDestroyEvent();
            }
        }
    }
}
