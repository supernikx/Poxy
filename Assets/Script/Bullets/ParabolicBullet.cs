using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe del proiettile stun
/// </summary>
public class ParabolicBullet : BulletBase
{
    [Header("ParabolicBullet Settings")]
    /// <summary>
    /// Moltiplicatore della velocità base dei proittili
    /// </summary>
    [SerializeField]
    float speedMultiplayer;
    [SerializeField]
    float gravity;

    private float travelTime;
    private float yVelocity;
    private float xVelocity;

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
            //Calcolo la direzione di movimento a parabola, tramite la gravità e il tempo trascorso
            Vector3 _movementDirection = new Vector3(xVelocity, (yVelocity - (gravity * travelTime)), 0);

            if (!Checkcollisions(_movementDirection))
            {
                //Calcolo la rotazione in base al movimento del proiettile e la applico
                float zRotation = Mathf.Atan2(_movementDirection.y, _movementDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, zRotation);

                transform.position += _movementDirection * Time.deltaTime;
                travelTime += Time.deltaTime;

                if (Vector3.Distance(shotPosition.position, transform.position) >= range)
                {
                    ObjectDestroyEvent();
                }
            }
        }
    }

    public override void Shot(float _speed, float _range, Transform _shootPosition, Vector3 _direction)
    {
        base.Shot(_speed * speedMultiplayer, _range, _shootPosition, _direction);

        //Calcolo la velocity sui 2 assi di movimento
        xVelocity = Mathf.Sqrt(speed) * Mathf.Cos(shotAngle * Mathf.Deg2Rad);
        yVelocity = Mathf.Sqrt(speed) * Mathf.Sin(shotAngle * Mathf.Deg2Rad);

        travelTime = 0;
    }
}
