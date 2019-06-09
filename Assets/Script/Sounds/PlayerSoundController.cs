using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : SoundControllerBase
{
    [Header("Player Movement Sound Settings")]
    [SerializeField]
    private AudioClipStruct walk;
    [SerializeField]
    private AudioClipStruct jump;
    [SerializeField]
    private AudioClipStruct landing;
    [SerializeField]
    private AudioClipStruct sticky;

    [Header("Parasite Sound Settings")]
    [SerializeField]
    private AudioClipStruct endParasite;
    [SerializeField]
    private AudioClipStruct parasite;

    [Header("Life Sound Settings")]
    [SerializeField]
    private AudioClipStruct bar;
    [SerializeField]
    private AudioClipStruct damageHit;
    [SerializeField]
    private AudioClipStruct acidHit;
    [SerializeField]
    private AudioClipStruct death;

    Player player;
    PlayerCollisionController collisionCtrl;

    public void Setup(Player _player)
    {
        player = _player;
        collisionCtrl = player.GetCollisionController();
    }

    public override void Init()
    {
        base.Init();

        PlayerMovementController.OnMovement += HandlePlayerMovement;
        PlayerMovementController.OnJump += HandlePlayerJump;
        PlayerCollisionController.OnPlayerLanding += HandlePlayerLanding;
        PlayerCollisionController.OnStickyCollision += HandleStickyCollision;
        PlayerParasiteController.OnPlayerParasite += HandlePlayerParasite;
        PlayerParasiteController.OnPlayerParasiteEnd += HandlePlayerParasiteEnd;
        PlayerHealthController.OnHealthChange += HandleHealthChange;
        player.OnDamageableCollision += HandleDamageableCollision;
        player.OnPlayerHit += HandlePlayerHit;
    }

    public void Death()
    {
        PlayAudioClip(death);
    }

    #region Handlers
    private void HandlePlayerLanding()
    {
        PlayAudioClip(landing);
    }

    private void HandleStickyCollision()
    {       
        PlayAudioClip(sticky);
    }

    private void HandlePlayerMovement(Vector3 _movementVelocity, CollisionInfo _collisions)
    {
        if (collisionCtrl.GetCollisionInfo().below && (_movementVelocity.x > 0.01f || _movementVelocity.x < -0.01f))
            PlayAudioClip(walk);
    }

    private void HandlePlayerJump()
    {
        PlayAudioClip(jump);
    }

    private void HandlePlayerParasite()
    {
        PlayAudioClip(parasite);
    }

    private void HandlePlayerParasiteEnd()
    {
        PlayAudioClip(endParasite);
    }

    private void HandleHealthChange(float health)
    {
        if ((health >= 64f && health <= 65f) || (health >= 24f && health <= 25f))
            PlayAudioClip(bar);
    }

    private void HandleDamageableCollision(IDamageable damageable)
    {
        if(damageable.DamageableType == DamageableType.Acid)
            PlayAudioClip(acidHit);
    }

    private void HandlePlayerHit()
    {
        PlayAudioClip(damageHit);
    }
    #endregion

    private void OnDisable()
    {
        PlayerMovementController.OnMovement -= HandlePlayerMovement;
        PlayerMovementController.OnJump -= HandlePlayerJump;
        PlayerCollisionController.OnPlayerLanding -= HandlePlayerLanding;
        PlayerCollisionController.OnStickyCollision -= HandleStickyCollision;
        PlayerParasiteController.OnPlayerParasite -= HandlePlayerParasite;
        PlayerParasiteController.OnPlayerParasiteEnd -= HandlePlayerParasiteEnd;
        PlayerHealthController.OnHealthChange -= HandleHealthChange;
        player.OnDamageableCollision -= HandleDamageableCollision;
        player.OnPlayerHit -= HandlePlayerHit;
    }
}
