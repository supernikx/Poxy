using UnityEngine;
using System.Collections;

public class DamageBullet : BulletBase
{
    protected override bool OnBulletCollision(Collision _collision)
    {
        if (ownerObject.tag == "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _collision.gameObject.GetComponent<IEnemy>().DamageHit(GetBulletDamage());
        }

        if (ownerObject.tag != "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = _collision.gameObject.GetComponent<Player>();
            if (player != null)
                player.GetHealthController().DamageHit(damage);
            else
                _collision.gameObject.GetComponent<IEnemy>().GetToleranceCtrl().AddTolerance(damage);

            if (player.OnPlayerHit != null)
                player.OnPlayerHit();
        }

        if (ownerObject.tag == "Player" && _collision.gameObject.layer == LayerMask.NameToLayer("Buttons"))
        {
            IButton _target = _collision.gameObject.GetComponent<IButton>();
            if (_target.GetTriggerType() == ButtonTriggerType.Shot)
                _target.Activate();
            else
                return false;
        }

        return base.OnBulletCollision(_collision);
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
}
