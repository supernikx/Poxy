using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            Time.timeScale = 0f;
            PlayerInputManager.SetCanReadInput(false);
            LevelManager.instance.GetUIGameplayManager().ToggleMenu(MenuType.EndGame);
        }
    }
}
