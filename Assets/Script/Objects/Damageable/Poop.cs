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

    [Header("Graphics Settings")]
    [SerializeField]
    private ParticleSystem poopParticle;

    private Animator anim;

    public void Setup()
    {
        anim = GetComponentInChildren<Animator>();
        poopParticle.Stop();
    }

    public void StartEffect()
    {
        anim.SetBool("Crawling", true);
        poopParticle.Play();
    }

    public void ResetEffect()
    {
        anim.SetBool("Crawling", false);
        poopParticle.Stop();
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
