using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poop : MonoBehaviour, IDamageable
{
    [SerializeField]
    private DamageableType type;
    public DamageableType DamageableType
    {
        get
        {
            return type;
        }

        set
        {
            type = value;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            if (PlayerCollisionController.DamageableNotification != null)
                PlayerCollisionController.DamageableNotification(this);
        }
    }
}
