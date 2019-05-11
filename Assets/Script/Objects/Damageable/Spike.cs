using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour, IDamageable
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
}
