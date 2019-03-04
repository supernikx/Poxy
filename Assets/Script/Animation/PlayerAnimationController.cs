using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator playerAnim;
    Animator animToUse;
    PlayerCollisionController collisionCtrl;

    public void Init(PlayerCollisionController _collisionCtrl)
    {
        animToUse = playerAnim = GetComponentInChildren<Animator>();
        collisionCtrl = _collisionCtrl;

        PlayerShotController.OnShot += ShotAnimation;
        PlayerMovementController.OnMovement += MovementAnimation;

        moving = false;
    }

    #region Head Animations
    private void HeadShot()
    {
        animToUse.SetTrigger("HeadShot");
    }

    private void HeadMovement(bool _enabled)
    {
        animToUse.SetBool("HeadWalk", _enabled);
    }

    private void HeadJump(bool _enabled)
    {
        animToUse.SetBool("HeadJump", _enabled);
    }
    #endregion

    #region Body Animations
    private void BodyShot()
    {
        animToUse.SetTrigger("BodyShot");
    }

    private void BodyMovement(bool _enabled)
    {
        animToUse.SetBool("BodyWalk", _enabled);
    }

    private void BodyJump(bool _enabled)
    {
        animToUse.SetBool("BodyJump", _enabled);
    }
    #endregion

    #region API
    public void ShotAnimation()
    {
        HeadShot();

        if (!moving)
        {
            BodyShot();
        }
    }

    bool moving;
    bool jumping;
    public void MovementAnimation(Vector3 _movementVelocity)
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

    #region Setter
    public void SetAnimator(Animator _anim)
    {
        animToUse = _anim;
        moving = false;
        jumping = false;
    }
    #endregion

    #region Getter
    public Animator GetPlayerAnimator()
    {
        return playerAnim;
    }

    public Animator GetCurrentAnimator()
    {
        return animToUse;
    }
    #endregion
    #endregion
}
