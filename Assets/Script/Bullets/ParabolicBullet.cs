using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe del proiettile stun
/// </summary>
public class ParabolicBullet : BulletBase
{
    [Header("ParabolicBullet Settings")]
    [SerializeField]
    float speedMultiplayer;
    [SerializeField]
    float gravity;

    private float firingAngle;
    private float flightDuration;
    private float elapse_time;
    private float Vy;
    private float Vx;

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

    private void Update()
    {
        if (CurrentState == State.InUse)
        {
            Vector3 _movementDirection = new Vector3(Vx, (Vy - (gravity * elapse_time)), 0);
            if (!Checkcollisions(_movementDirection))
            {
                float zRotation = Mathf.Atan2(_movementDirection.y, _movementDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, zRotation);

                transform.position += _movementDirection * Time.deltaTime;
                elapse_time += Time.deltaTime;

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

        firingAngle = shotAngle;

        float target_Distance = speed * (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        Vx = Mathf.Sqrt(speed) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        Vy = Mathf.Sqrt(speed) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        elapse_time = 0;
    }

    public class ThrowSimulation : MonoBehaviour
    {
        public Transform Target;
        public float firingAngle = 45.0f;
        public float gravity = 9.8f;

        public Transform Projectile;
        private Transform myTransform;

        void Awake()
        {
            myTransform = transform;
        }

        IEnumerator SimulateProjectile()
        {
            // Short delay added before Projectile is thrown
            yield return new WaitForSeconds(1.5f);

            // Move projectile to the position of throwing object + add some offset if needed.
            Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

            // Calculate distance to target
            float target_Distance = Vector3.Distance(Projectile.position, Target.position);

            // Calculate the velocity needed to throw the object to the target at specified angle.
            float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

            // Extract the X  Y componenent of the velocity
            float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
            float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

            // Calculate flight time.
            float flightDuration = target_Distance / Vx;

            // Rotate projectile to face the target.
            Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);

            float elapse_time = 0;

            while (elapse_time < flightDuration)
            {
                Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

                elapse_time += Time.deltaTime;

                yield return null;
            }
        }
    }
}
