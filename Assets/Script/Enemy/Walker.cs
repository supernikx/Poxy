using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Walker : EnemyBase
{
    private float velocityXSmoothing;

    private float HorizontalVelocityRoaming()
    {
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

    private float HorizontalVelocityAlert()
    {
        if (transform.position.x > viewCtrl.GetPlayerPosition().x)
        {
            direction = -1;
        }
        else if (transform.position.x < viewCtrl.GetPlayerPosition().x)
        {
            direction = 1;
        }

        return direction * horizontalVelocity;
    }

    #region API

    public override void MoveRoaming()
    {
        //transform.DOPath(GetWaypoints(), 5).SetOptions(true, AxisConstraint.Y).SetLoops(-1).SetEase(Ease.Linear);
        Vector3 movementVelocity = movementCtrl.GravityCheck();
        movementVelocity.x = Mathf.SmoothDamp(0, HorizontalVelocityRoaming(), ref velocityXSmoothing, (collisionCtrl.collisions.below ? AccelerationTimeOnGround : AccelerationTimeOnAir));
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

    public override bool AlertActions()
    {
        Vector3 target = viewCtrl.GetPlayerPosition();

        if (target != Vector3.zero)
        {
            // if check shoot radius
            //      shoot
            //      return true

            Vector3 movementVelocity = movementCtrl.GravityCheck();
            movementVelocity.x = Mathf.SmoothDamp(0, HorizontalVelocityAlert(), ref velocityXSmoothing, (collisionCtrl.collisions.below ? AccelerationTimeOnGround : AccelerationTimeOnAir));
            transform.Translate(collisionCtrl.CheckMovementCollisions(movementVelocity * Time.deltaTime));

            return true;
        }
        
        return false;
    }

    public override void Stop()
    {
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
    public override CapsuleCollider GetCollider()
    {
        return GetComponent<CapsuleCollider>();
    }
    #endregion
    #endregion

}
