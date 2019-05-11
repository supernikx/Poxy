using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    DamageableType DamageableType { set; get; }
}

public enum DamageableType
{
    Spike,
    Acid,
    Poop,
}
