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

    [Header("Player Shot Sound Settings")]
    [SerializeField]
    private AudioClipStruct stunBullet;
    [SerializeField]
    private AudioClipStruct stickyBullet;
    [SerializeField]
    private AudioClipStruct parabolicBullet;

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
        PlayerShotController.OnShotDone += HandleOnShot;
    }

    #region Handlers
    private void HandlePlayerLanding()
    {
        PlayAudioClip(landing);
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

    private void HandleOnShot(ObjectTypes _bullet)
    {
        switch (_bullet)
        {
            case ObjectTypes.StunBullet:
                PlayAudioClip(stunBullet);
                break;
            case ObjectTypes.ParabolicBullet:
                PlayAudioClip(parabolicBullet);
                break;
            case ObjectTypes.StickyBullet:
                PlayAudioClip(stickyBullet);
                break;
        }
    }

    #endregion

    private void OnDisable()
    {
        PlayerMovementController.OnMovement -= HandlePlayerMovement;
        PlayerMovementController.OnJump -= HandlePlayerJump;
        PlayerCollisionController.OnPlayerLanding -= HandlePlayerLanding;
        PlayerShotController.OnShotDone -= HandleOnShot;
    }
}
