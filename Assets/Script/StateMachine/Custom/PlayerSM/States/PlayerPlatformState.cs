using UnityEngine;
using System.Collections;
using StateMachine.PlayerSM;
using System;

public class PlayerPlatformState : PlayerSMStateBase
{
    [SerializeField]
    private float parasiteTimeDelay;

    private CameraManager cam;
    private bool healthMax;
    private LaunchingPlatform parasitePlatform;

    public override void Enter()
    {
        cam = LevelManager.instance.GetCameraManager();
        parasitePlatform = context.parasite as LaunchingPlatform;

        parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar += HandleOnMaxTolleranceBar;
        PlayerInputManager.OnShotPressed += HandleParasiteExit;
        PlayerInputManager.OnParasitePressed += HandleParasiteExit;
        context.player.OnDamageableCollision += HandleDamageableCollision;
        PlayerInputManager.DelayParasiteButtonPress(parasiteTimeDelay);

        parasitePressed = false;
        healthMax = false;

        parasitePlatform.gameObject.transform.parent = context.player.transform;
        parasitePlatform.gameObject.transform.localPosition = Vector3.zero;

        context.player.GetShotController().SetCrossairDistance(3f);
        context.player.GetShotController().SetCanShoot(false);
        context.player.StopImmunityCoroutine();
        context.player.gameObject.layer = LayerMask.NameToLayer("PlayerImmunity");

        cam.SetPlatformCamera(parasitePlatform.GetObjectToFollow());
    }

    public override void Tick()
    {
        if (context.player.GetHealthController().GainHealthOverTime() && !healthMax)
        {
            healthMax = true;
            if (context.player.OnPlayerMaxHealth != null)
                context.player.OnPlayerMaxHealth();
        }

        parasitePlatform.RotationUpdate(PlayerInputManager.GetAimVector());
    }

    private bool parasitePressed;
    private void HandleParasiteExit()
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

    private void HandleDamageableCollision(IDamageable _damageable)
    {
        context.player.OnDamageableCollision -= HandleDamageableCollision;

        switch (_damageable.DamageableType)
        {
            case DamageableType.Poop:
                context.player.Death();
                break;
        }
    }

    public override void Exit()
    {
        parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar -= HandleOnMaxTolleranceBar;
        PlayerInputManager.OnShotPressed -= HandleParasiteExit;
        PlayerInputManager.OnParasitePressed -= HandleParasiteExit;
        context.player.OnDamageableCollision -= HandleDamageableCollision;

        context.player.GetParasiteController().SetParasite(null);

        context.player.GetCollisionController().CalculateNormalCollision();

        context.player.GetShotController().SetCrossairDistance(1.5f);
        context.player.GetShotController().SetCanShoot(true);
        context.player.GetMovementController().SetCanMove(true);

        parasitePressed = false;
        healthMax = false;

        cam.SetPlayerCamera();
        context.player.gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void OnDestroy()
    {
        if (parasitePlatform != null)
            parasitePlatform.GetToleranceCtrl().OnMaxTolleranceBar -= HandleOnMaxTolleranceBar;
        PlayerInputManager.OnShotPressed -= HandleParasiteExit;
        PlayerInputManager.OnParasitePressed -= HandleParasiteExit;
    }
}
