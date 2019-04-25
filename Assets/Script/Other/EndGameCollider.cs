using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        LevelManager.instance.GetUIGameplayManager().ToggleMenu(MenuType.EndGame);
    }
}
