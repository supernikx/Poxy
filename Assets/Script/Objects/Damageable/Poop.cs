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

    private GeneralSoundController soundCtrl;
    private Animator anim;

    public void Setup()
    {
        anim = GetComponentInChildren<Animator>();
        soundCtrl = GetComponentInChildren<GeneralSoundController>();

        poopParticle.Stop();
    }

    public void StartEffect()
    {
        soundCtrl.PlayClip();
        anim.SetBool("Crawling", true);
        poopParticle.Play();
    }

    public void ResetEffect()
    {
        soundCtrl.StopClip();
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
