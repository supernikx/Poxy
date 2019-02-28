using UnityEngine;
using System.Collections;

public class DamageBullet : BulletBase
{
    protected override bool OnBulletCollision(RaycastHit _collisionInfo)
    {
        if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _collisionInfo.transform.gameObject.GetComponent<IEnemy>().DamageHit(GetBulletDamage());
        }

        if (ownerObject.tag != "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = _collisionInfo.transform.gameObject.GetComponent<Player>();
            if (player != null)
                player.GetHealthController().DamageHit(damage);
            else
                _collisionInfo.transform.gameObject.GetComponent<IEnemy>().GetToleranceCtrl().AddTolerance(damage);
        }

        if (ownerObject.tag == "Player" && _collisionInfo.transform.gameObject.layer == LayerMask.NameToLayer("Buttons"))
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
