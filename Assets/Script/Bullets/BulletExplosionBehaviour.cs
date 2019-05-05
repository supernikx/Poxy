using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletExplosionBehaviour : MonoBehaviour
{
    [SerializeField]
    float damageMultyplier;
    [SerializeField]
    float explosionRange;
    [SerializeField]
    LayerMask enemyLayer;
    [SerializeField]
    LayerMask playerLayer;

    public void Explode(GameObject _owner, float _damage)
    {
        if (_owner.tag == "Player")
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRange, enemyLayer);
            foreach (Collider collision in collisions)
            {
                IEnemy enemyHit = collision.transform.gameObject.GetComponent<IEnemy>();
                if (enemyHit != null)
                    enemyHit.DamageHit(_damage * damageMultyplier);
            }
        }
        else
        {
            Collider[] collisions = Physics.OverlapSphere(transform.position, explosionRange, playerLayer);
            if (collisions == null || collisions.Length == 0)
                return;

            Player player = collisions[0].transform.gameObject.GetComponent<Player>();
            if (player != null)
                player.GetHealthController().DamageHit(_damage * damageMultyplier);
            else
            {
                IEnemy enemyHit = collisions[0].transform.gameObject.GetComponent<IEnemy>();
                if (enemyHit != null)
                {
                    player = enemyHit.gameObject.GetComponentInParent<Player>();
                    enemyHit.GetToleranceCtrl().AddTolerance(_damage * damageMultyplier);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
