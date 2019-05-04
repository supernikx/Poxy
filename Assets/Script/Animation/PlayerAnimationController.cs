using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator playerAnim;
    Player player;
    PlayerCollisionController collisionCtrl;
    EnemyAnimationController controllerToUse;

    public void Init(Player _player, PlayerCollisionController _collisionCtrl)
    {
        playerAnim = GetComponentInChildren<Animator>();
        collisionCtrl = _collisionCtrl;
        player = _player;
        controllerToUse = null;

        PlayerShotController.OnShot += ShotAnimation;
        PlayerMovementController.OnMovement += MovementAnimation;
        player.OnPlayerHit += HitAnimation;

        moving = false;
    }

    #region Head Animations
    private void HeadShot()
    {
        playerAnim.SetTrigger("HeadShot");
    }

    private void HeadMovement(bool _enabled)
    {
        playerAnim.SetBool("HeadWalk", _enabled);
    }

    private void HeadJump(bool _enabled)
    {
        playerAnim.SetBool("HeadJump", _enabled);
    }
    #endregion

    #region Body Animations
    private void BodyShot()
    {
        playerAnim.SetTrigger("BodyShot");
    }

    private void BodyMovement(bool _enabled)
    {
        playerAnim.SetBool("BodyWalk", _enabled);
    }

    private void BodyJump(bool _enabled)
    {
        playerAnim.SetBool("BodyJump", _enabled);
    }
    #endregion

    #region API
    bool moving;
    bool jumping;
    public void MovementAnimation(Vector3 _movementVelocity)
    {
        if (controllerToUse == null)
        {
            if (collisionCtrl.GetCollisionInfo().below)
            {
                if (jumping)
                {
                    jumping = false;
                    BodyJump(false);
                    HeadJump(false);
                }

                if (!moving && (_movementVelocity.x < -0.001f || _movementVelocity.x > 0.001f))
                {
                    moving = true;
                    HeadMovement(true);
                    BodyMovement(true);
                }
                else if (moving && (_movementVelocity.x > -0.001f && _movementVelocity.x < 0.001f))
                {
                    moving = false;
                    HeadMovement(false);
                    BodyMovement(false);
                }
            }
            else if (!collisionCtrl.GetCollisionInfo().below && !jumping)
            {
                jumping = true;
                BodyJump(true);
                HeadJump(true);
            }
        }
        else
        {
            controllerToUse.MovementAnimation(_movementVelocity);
        }
    }

    #region Shot
    Action shotAnimationCallback;
    public void ShotAnimation(Action _OnShotAnimationEndCallback)
    {
        if (controllerToUse == null)
        {
            shotAnimationCallback = _OnShotAnimationEndCallback;

            HeadShot();
            if (!moving)
            {
                BodyShot();
            }
        }
        else
        {
            controllerToUse.ShotAnimation(_OnShotAnimationEndCallback);
        }
    }

    private void HandleShotAnimationEnd()
    {
        if (shotAnimationCallback != null)
            shotAnimationCallback();

        shotAnimationCallback = null;
    }
    #endregion

    #region Hit
    private void HitAnimation()
    {
        if (controllerToUse != null)
            controllerToUse.HitAnimation();
    }
    #endregion

    #region Setter
    public void SetAnimatorController(EnemyAnimationController _controller)
    {
        controllerToUse = _controller;
        moving = false;
        jumping = false;
    }
    #endregion

    #region Getter
    public Animator GetPlayerAnimator()
    {
        return playerAnim;
    }
    #endregion
    #endregion

    private void OnDisable()
    {
        PlayerShotController.OnShot -= ShotAnimation;
        PlayerMovementController.OnMovement -= MovementAnimation;
        player.OnPlayerHit -= HitAnimation;
    }
}
