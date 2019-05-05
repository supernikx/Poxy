using System;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator anim;
    private EnemyBase enemy;

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

    private void HeadSticky(bool _enabled)
    {
        anim.SetBool("HeadSticky", _enabled);
    }

    private void HeadStun(bool _stun)
    {
        if (_stun)
        {
            anim.SetTrigger("HeadStun");
            anim.ResetTrigger("HeadEndStun");
        }
        else
        {
            anim.SetTrigger("HeadEndStun");
            anim.ResetTrigger("HeadStun");
        }
    }

    private void HeadHit()
    {
        anim.SetTrigger("HeadHit");
    }

    private void HeadDeath()
    {
        anim.SetTrigger("HeadDeath");
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

    private void BodySticky(bool _enabled)
    {
        anim.SetBool("BodySticky", _enabled);
    }

    private void BodyStun(bool _stun)
    {
        if (_stun)
        {
            anim.SetTrigger("BodyStun");
            anim.ResetTrigger("BodyEndStun");

        }
        else
        {
            anim.SetTrigger("BodyEndStun");
            anim.ResetTrigger("BodyStun");
        }
    }

    private void BodyHit()
    {
        anim.SetTrigger("BodyHit");
    }

    private void BodyDeath()
    {
        anim.SetTrigger("BodyDeath");
    }
    #endregion

    #region API
    public void Init(EnemyBase _enemy)
    {
        anim = GetComponentInChildren<Animator>();
        enemy = _enemy;

        enemy.OnEnemyShot += ShotAnimation;
        enemy.OnEnemyHit += HitAnimation;

        ResetAnimator();
    }

    /// <summary>
    /// Funzione che ritorna l'animator
    /// </summary>
    public Animator GetAnimator()
    {
        return anim;
    }

    #region Movement
    bool moving;
    bool jumping;
    bool sticky;
    public void MovementAnimation(Vector3 _movementVelocity, CollisionInfo _collisions)
    {
        if (stun)
            return;

        if (_collisions.StickyCollision())
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

            if (!sticky)
            {
                sticky = true;
                HeadSticky(true);
                BodySticky(true);
            }
        }
        else if (_collisions.below)
        {
            if (sticky)
            {
                sticky = false;
                HeadSticky(false);
                BodySticky(false);
            }

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
        else if (!_collisions.below && !jumping)
        {
            jumping = true;
            BodyJump(true);
            HeadJump(true);
        }
    }
    #endregion

    #region Shot
    Action shotAnimationCallback;
    public void ShotAnimation(Action _OnShotAnimationEndCallback)
    {
        shotAnimationCallback = _OnShotAnimationEndCallback;
        HeadShot();

        if (!moving)
        {
            BodyShot();
        }
    }

    public void HandleShotAnimationEnd()
    {
        if (shotAnimationCallback != null)
            shotAnimationCallback();

        shotAnimationCallback = null;
    }
    #endregion

    #region Hit
    bool hittable;
    int hitEndAnimationCount;
    public void HitAnimation()
    {
        if (!hittable || stun)
            return;

        hittable = false;
        hitEndAnimationCount = 0;
        HeadHit();
        BodyHit();
    }

    public void HandleHitAnimationEnd()
    {
        Debug.Log("End");
        hitEndAnimationCount++;
        if (hitEndAnimationCount == 2)
            hittable = true;
    }
    #endregion

    #region DeathAnimation
    Action deathAnimationCallback;
    public void DeathAnimation(Action _OnDeathAnimationEndCallback)
    {
        deathAnimationCallback = _OnDeathAnimationEndCallback;
        HeadDeath();
        BodyDeath();
    }

    public void HandleDeathAnimationEnd()
    {
        if (deathAnimationCallback != null)
            deathAnimationCallback();

        deathAnimationCallback = null;
    }
    #endregion

    public void ResetAnimator()
    {
        hittable = true;
        hitEndAnimationCount = 0;

        stun = false;
        HeadStun(stun);
        BodyStun(stun);

        jumping = false;
        BodyJump(false);
        HeadJump(false);

        moving = false;
        HeadMovement(false);
        BodyMovement(false);

        sticky = false;
        HeadSticky(false);
        BodySticky(false);
    }

    bool stun;
    public void Stun(bool _stun)
    {
        stun = _stun;
        HeadStun(stun);
        BodyStun(stun);
    }
    #endregion

    private void OnDisable()
    {
        enemy.OnEnemyShot -= ShotAnimation;
        enemy.OnEnemyHit -= HitAnimation;
    }
}
