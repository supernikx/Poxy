using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalState : StateMachineBehaviour {

    Player player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player>();
        if (player != null)
        {
            player.GetShootController().SetCanShoot(true);
            player.GetMovementController().SetCanMove(true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
