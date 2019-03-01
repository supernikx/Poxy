using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator anim;
    private EnemyCollisionController collisionCtrl;

    #region Head Animations
    private void HeadShot()
    {
        anim.SetTrigger("HeadShot");
    }

    private void HeadMovement(bool _enabled)
    {
        anim.SetBool("HeadWalk", _enabled);
    }

    private void HeadJump(bool _enabled)
    {
        anim.SetBool("HeadJump", _enabled);
    }

    private void HeadStun(bool _stun)
    {
        if (_stun)
            anim.SetTrigger("HeadStun");
        else
            anim.SetTrigger("HeadEndStun");
    }
    #endregion

    #region Body Animations
    private void BodyShot()
    {
        anim.SetTrigger("BodyShot");
    }

    private void BodyMovement(bool _enabled)
    {
        anim.SetBool("BodyWalk", _enabled);
    }

    private void BodyJump(bool _enabled)
    {
        anim.SetBool("BodyJump", _enabled);
    }

    private void BodyStun(bool _stun)
    {
        if (_stun)
            anim.SetTrigger("BodyStun");
        else
            anim.SetTrigger("BodyEndStun");
    }
    #endregion

    #region API
    public void Init(EnemyCollisionController _collisionCtrl)
    {
        anim = GetComponent<Animator>();
        collisionCtrl = _collisionCtrl;
    }

    /// <summary>
    /// Funzione che ritorna l'animator
    /// </summary>
    public Animator GetAnimator()
    {
        return anim;
    }

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
        if (stun)
            return;

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

    public void ResetAnimator()
    {
        stun = false;
        HeadStun(stun);
        BodyStun(stun);

        jumping = false;
        BodyJump(false);
        HeadJump(false);

        moving = false;
        HeadMovement(false);
        BodyMovement(false);
    }

    bool stun;
    public void Stun(bool _stun)
    {
        stun = _stun;
        HeadStun(stun);
        BodyStun(stun);
    }
    #endregion
}
