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

        // ?? context.player.OnEnemyCollision += OnEnemyCollision;
        parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar += OnMaxTolleranceBar;

        parasitePressed = false;
        healthMax = false;

        parasitePlatform.gameObject.transform.parent = context.player.transform;
        parasitePlatform.gameObject.transform.localPosition = Vector3.zero;

        context.player.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");
    }

    private bool parasitePressed;
    public override void Tick()
    {
        if (Input.GetButtonDown("Parasite") || Input.GetButtonDown("Jump") && !parasitePressed)
        {
            parasitePressed = true;
            context.player.StartNormalCoroutine();
        }

        if (context.player.GetHealthController().GainHealthOverTime() && !healthMax)
        {
            healthMax = true;
            if (context.player.OnPlayerMaxHealth != null)
                context.player.OnPlayerMaxHealth();
        }
    }

    public override void Exit()
    {
        //context.player.OnEnemyCollision -= OnEnemyCollision;
        if (parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar != null)
            parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar -= OnMaxTolleranceBar;

        context.player.GetParasiteController().SetParasite(null);

        context.player.GetCollisionController().CalculateNormalCollision();

        context.player.GetShotController().SetCanShoot(true);
        context.player.GetMovementController().SetCanMove(true);
        parasitePressed = false;
        healthMax = false;

        context.player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnMaxTolleranceBar()
    {
        context.player.StartNormalCoroutine();
    }

}
