using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Walker : EnemyBase
{
    private float HorizontalMoving()
    {
        int direction = 0;
        if (transform.position.x > path[nextWaypoint] + WaypointOffset)
        {
            direction = -1;
        }
        else if (transform.position.x < path[nextWaypoint] - WaypointOffset)
        {
            direction = 1;
        }
        else
        {
            nextWaypoint += 1;
            if (path.Length == nextWaypoint)
            {
                nextWaypoint = 0;
            }
        }

        return direction * horizontalVelocity;
    }

    #region API
    public override void Move()
    {
        //transform.DOPath(GetWaypoints(), 5).SetOptions(true, AxisConstraint.Y).SetLoops(-1).SetEase(Ease.Linear);
        Vector3 movementVelocity = movementCtrl.GravityCheck();
        movementVelocity.x = HorizontalMoving();
        transform.Translate(collisionCtrl.CheckMovementCollisions(movementVelocity * Time.deltaTime));
        if (collisionCtrl.collisions.right || collisionCtrl.collisions.left)
        {
            nextWaypoint += 1;
            if (path.Length == nextWaypoint)
            {
                nextWaypoint = 0;
            }
        }
    }

    public override void Stop()
    {
        DOTween.Kill(this.transform, false);
    }

    #region Getters
    /// <summary>
    /// Get stun duration
    /// </summary>
    public override int GetStunDuration()
    {
        return stunDuration;
    }

    /// <summary>
    /// Get Death Duration
    /// </summary>
    public override int GetDeathDuration()
    {
        return deathDuration;
    }

    /// <summary>
    /// Get Graphics Reference
    /// </summary>
    public override GameObject GetGraphics()
    {
        return graphics;
    }

    /// <summary>
    /// Get Collider Reference
    /// </summary>
    public override BoxCollider GetCollider()
    {
        return GetComponent<BoxCollider>();
    }
    #endregion
    #endregion

}
