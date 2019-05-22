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
    [SerializeField]
    private ParticleSystem bulletParticle;
    [SerializeField]
    private ParticleSystem bulletExplosionParticle;

    private float travelTime;
    private float yVelocity;
    private float xVelocity;

    private bool canMove = true;

    protected override bool OnBulletCollision(Collider _collider)
    {
        if (ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            IEnemy enemyHit = _collider.gameObject.GetComponent<IEnemy>();
            if (enemyHit != null)
            {
                enemyHit.DamageHit(GetBulletDamage());
                enemyHit.ApplyKnockback(shotDirection.Value, enemyKnockbackForce);
                EnemyBase enemyBase = (enemyHit as EnemyBase);
                if (enemyBase != null && enemyBase.OnEnemyHit != null)
                    enemyBase.OnEnemyHit();
            }
        }

        else if (ownerObject.tag != "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = _collider.gameObject.GetComponent<Player>();
            if (player != null)
                player.GetHealthController().DamageHit(damage);
            else
            {
                IEnemy enemyHit = _collider.gameObject.GetComponent<IEnemy>();
                if (enemyHit != null)
                {
                    player = enemyHit.gameObject.GetComponentInParent<Player>();
                    enemyHit.GetToleranceCtrl().AddTolerance(damage);
                }
            }

            if (player.OnPlayerHit != null)
                player.OnPlayerHit();
        }

        else if (ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            IButton _target = _collider.gameObject.GetComponent<IButton>();
            if (_target.GetTriggerType() == ButtonTriggerType.Shot)
                _target.Activate();
            else
                return false;
        }

        return base.OnBulletCollision(_collider);
    }

    protected override void Move()
    {
        if (canMove)
        {
            Vector3 _movementDirection = new Vector3(xVelocity, (yVelocity - (gravity * travelTime)), 0);
            //Calcolo la rotazione in base al movimento del proiettile e la applico
            float zRotation = Mathf.Atan2(_movementDirection.y, _movementDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0.0f, 0.0f, zRotation);

            transform.position += _movementDirection * Time.deltaTime;
            travelTime += Time.deltaTime;

            if (Vector3.Distance(shotPosition, transform.position) >= range)
            {
                ObjectDestroyEvent();
            }
        }
    }

    public override void Shot(float _damage, float _speed, float _range, Vector3 _shootPosition, Vector3 _direction)
    {
        base.Shot(_damage, Mathf.Sqrt(_speed * speedMultiplayer), _range, _shootPosition, _direction);

        //Calcolo la velocity sui 2 assi di movimento in base all'angolo di mira
        xVelocity = speed * Mathf.Cos(shotAngle * Mathf.Deg2Rad);
        yVelocity = speed * Mathf.Sin(shotAngle * Mathf.Deg2Rad);

        travelTime = 0;
    }

    public override void Shot(float _damage, float _speed, float _range, Vector3 _shotPosition, Transform _target)
    {
        base.Shot(_damage, Mathf.Sqrt(_speed * speedMultiplayer), _range, _shotPosition, _target);

        //calcolo l'offset sui 2 assi
        float x = targetPosition.Value.x - transform.position.x;
        float y = targetPosition.Value.y - transform.position.y;

        //Calcolo la discriminante
        float b = speed * speed - y * gravity;
        float discriminant = b * b - gravity * gravity * (x * x + y * y);

        //Controllo se posso raggiungere il target con la mia velocità e gravità
        if (discriminant < 0)
        {
            Debug.LogWarning("Speed is to low for reach the target");
            ObjectDestroyEvent();
            return;
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Tempo di impatto per un tiro diretto
        float T_min = Mathf.Sqrt((b - discRoot) * 2 / (gravity * gravity));

        // Tempo d'impatto per il tiro più alto possibile
        float T_max = Mathf.Sqrt((b + discRoot) * 2 / (gravity * gravity));

        //Faccio una media dei 2 valori
        float T = (T_max - T_min) * 0.5f + T_min;

        //Calcolo la velocità sui 2 assi di movimento
        xVelocity = x / T;
        yVelocity = y / T + T * gravity / 2;

        travelTime = 0;

        //Calcolo l'angolo (ora non serve lo tengo come template)
        //float angle = Mathf.Atan2(yVelocity, xVelocity) * Mathf.Rad2Deg;
    }

    public bool CheckShotRange(Vector3 _target, Vector3 _shotPosition, float _speed)
    {
        _speed = Mathf.Sqrt(_speed * speedMultiplayer);

        //calcolo l'offset sui 2 assi
        float x = _target.x - _shotPosition.x;
        float y = _target.y - _shotPosition.y;

        //Calcolo la discriminante
        float b = _speed * _speed - y * gravity;
        float discriminant = b * b - gravity * gravity * (x * x + y * y);

        //Controllo se posso raggiungere il target con la mia velocità e gravità
        if (discriminant < 0)
            return false;

        return true;
    }

    #region Spawn/Destroy
    protected override void ObjectDestroyEvent()
    {
        canMove = false;
        bulletParticle.Stop();
        bulletExplosionParticle.transform.parent = null;
        bulletExplosionParticle.gameObject.transform.position = transform.position + new Vector3(0, 0.65f, 0);
        bulletExplosionParticle.Play();
        StartCoroutine(CBulletExplosion());
        base.ObjectDestroyEvent();
    }

    private IEnumerator CBulletExplosion()
    {
        yield return new WaitForSeconds(bulletExplosionParticle.main.duration);

        bulletExplosionParticle.transform.parent = transform.GetChild(1).transform;
        bulletExplosionParticle.transform.position = Vector3.zero;
    }

    protected override void ObjectSpawnEvent()
    {
        bulletParticle.Play();
        canMove = true;

        base.ObjectSpawnEvent();
    }
    #endregion
}
