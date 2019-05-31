using UnityEngine;
using System.Collections;

public class DamageBullet : BulletBase
{    [Header("Damage Bullet Settings")]
    [SerializeField]
    private ParticleSystem bulletParticle;

    protected override bool OnBulletCollision(Collider _collider)
    {
        if (ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _collider.gameObject.GetComponent<IEnemy>().DamageHit(GetBulletDamage());
        }

        if (ownerObject.tag != "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = _collider.gameObject.GetComponent<Player>();
            if (player != null)
                player.GetHealthController().DamageHit(damage);
            else
                _collider.gameObject.GetComponent<IEnemy>().GetToleranceCtrl().AddTolerance(damage);

            if (player != null && player.OnPlayerHit != null)
                player.OnPlayerHit();
        }

        if (ownerObject.tag == "Player" && _collider.gameObject.layer == LayerMask.NameToLayer("Buttons"))
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
        Vector3 _movementDirection = transform.right * speed;
        transform.position += _movementDirection * Time.deltaTime;
        if (Vector3.Distance(shotPosition, transform.position) >= range)
        {
            ObjectDestroyEvent();
        }
    }

    #region Spawn/Destroy
    protected override void ObjectDestroyEvent()
    {
        bulletParticle.Stop();
        base.ObjectDestroyEvent();
    }

    protected override void ObjectSpawnEvent()
    {
        bulletParticle.Play();
        base.ObjectSpawnEvent();
    }
    #endregion
}
