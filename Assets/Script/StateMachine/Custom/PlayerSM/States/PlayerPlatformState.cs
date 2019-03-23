using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;

public class PlayerPlatformState : PlayerSMStateBase
{
    private bool healthMax;
    private LaunchingPlatform parasitePlatform;

    public override void Enter()
    {
        parasitePlatform = context.parasite as LaunchingPlatform;

        parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar += HandleOnMaxTolleranceBar;
        PlayerInputManager.OnParasitePressed += HandleOnPlayerParasitePressed;
        PlayerInputManager.OnJumpPressed += HandleOnPlayerParasitePressed;

        parasitePressed = false;
        healthMax = false;

        parasitePlatform.gameObject.transform.parent = context.player.transform;
        parasitePlatform.gameObject.transform.localPosition = Vector3.zero;

        context.player.GetShotController().SetCanShoot(false);
        context.player.GetShotController().SetCanAim(false);
        context.player.StopImmunityCoroutine();
        context.player.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");
    }

    public override void Tick()
    {
        if (context.player.GetHealthController().GainHealthOverTime() && !healthMax)
        {
            healthMax = true;
            if (context.player.OnPlayerMaxHealth != null)
                context.player.OnPlayerMaxHealth();
        }
    }

    private bool parasitePressed;
    private void HandleOnPlayerParasitePressed()
    {
        if (parasitePressed)
            return;

        parasitePressed = true;
        context.player.StartNormalCoroutine();
    }

    private void HandleOnMaxTolleranceBar()
    {
        context.player.StartNormalCoroutine();
    }

    public override void Exit()
    {
        parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar -= HandleOnMaxTolleranceBar;
        PlayerInputManager.OnParasitePressed -= HandleOnPlayerParasitePressed;
        PlayerInputManager.OnJumpPressed -= HandleOnPlayerParasitePressed;

        context.player.GetParasiteController().SetParasite(null);

        context.player.GetCollisionController().CalculateNormalCollision();

        context.player.GetShotController().SetCanAim(true);
        context.player.GetShotController().SetCanShoot(true);
        context.player.GetMovementController().SetCanMove(true);
        parasitePressed = false;
        healthMax = false;

        context.player.gameObject.layer = LayerMask.NameToLayer("Player");
    }
}
