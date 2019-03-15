using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateMachine.EnemySM;

public class EnemyAlertState : EnemySMStateBase
{
    private IEnemy enemy;
    private EnemyViewController viewCtrl;
    private Transform target;
    private float rotationY;
    private Vector3 direction;

    public override void Enter()
    {
        enemy = context.enemy;
        viewCtrl = enemy.GetViewCtrl();
        AlertSetup();
    }

    private void AlertSetup()
    {
        enemy.EnemyAlertState();
        target = viewCtrl.FindPlayer();

        direction = (target.position - enemy.gameObject.transform.position).normalized;
        if (direction != enemy.gameObject.transform.right)
        {
            rotationY = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 rotationVector = Vector3.zero;
            rotationVector.y = rotationY;
            enemy.gameObject.transform.eulerAngles = rotationVector;
        }
    }

    private void MoveAlert(Transform _target)
    {
        Vector3 movementVector = Vector3.zero;
        Vector3 movementVelocity = movementVector;

        //Rotazione Nemico
        Vector3 targetPosition = new Vector3(target.position.x, enemy.gameObject.transform.position.y, enemy.gameObject.transform.position.z);
        Vector3 direction = (targetPosition - enemy.gameObject.transform.position).normalized;
        if (direction.x != 0)
        {
            rotationY = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 rotationVector = Vector3.zero;
            rotationVector.y = rotationY;
            enemy.gameObject.transform.eulerAngles = rotationVector;
        }

        if (enemy.CheckShot(target))
        {
            enemy.Shot(target);

            if (enemy.GetCollisionCtrl().GetCollisionInfo().StickyCollision())
            {
                return;
            }
            movementVelocity = enemy.GetMovementCtrl().MovementCheck(movementVector);
        }
        else
        {
            if (enemy.GetCollisionCtrl().GetCollisionInfo().StickyCollision())
            {
                return;
            }

            //Movimento Nemico                
            movementVector.x = enemy.GetMovementSpeed();
            movementVelocity = enemy.GetMovementCtrl().MovementCheck(movementVector);
        }

        enemy.GetAnimationController().MovementAnimation(movementVelocity);
    }

    public override void Tick()
    {
        target = viewCtrl.FindPlayer();
        if (target == null)
            context.EndAlertCallback();
        else
            MoveAlert(target);
    }

    public override void Exit()
    {

    }

}
