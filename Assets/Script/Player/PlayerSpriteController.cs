using UnityEngine;
using System.Collections;

public class PlayerSpriteController : MonoBehaviour
{
    [Header("Exclamation Marks")]
    [SerializeField]
    private GameObject exclamationSprite;
    [SerializeField]
    private float exclamationTriggerLifeTotal;

    private Player player;

    #region API
    public void Init(Player _player)
    {
        player = _player;
        exclamationSprite.SetActive(false);

        // Parasite Events
        PlayerParasiteController.OnPlayerParasite += HandleOnPlayerParasite;
        PlayerParasiteController.OnPlayerParasiteEnd += HandleOnPlayerParasiteEnd;

        // Exclamation Events
        PlayerHealthController.OnHealthChange += HandleOnHealthChange;
    }
    #endregion

    #region Exclamation
    private void HandleOnHealthChange(float _health)
    {
        if (_health <= exclamationTriggerLifeTotal && !exclamationSprite.activeInHierarchy)
        {
            exclamationSprite.SetActive(true);
        }
        else if (_health > exclamationTriggerLifeTotal && exclamationSprite.activeInHierarchy)
        {
            exclamationSprite.SetActive(false);
        }

        if (_health <= 0)
        {
            exclamationSprite.SetActive(false);
        }
    }

    public bool IsExclamationActive()
    {
        return exclamationSprite.activeInHierarchy;
    }
    #endregion

    #region Handlers
    private void HandleOnPlayerParasite()
    {
        PlayerHealthController.OnHealthChange -= HandleOnHealthChange;
        exclamationSprite.SetActive(false);
    }

    private void HandleOnPlayerParasiteEnd()
    {
        PlayerHealthController.OnHealthChange += HandleOnHealthChange;
    }
    #endregion

    private void OnDisable()
    {
        // Parasite Events
        PlayerParasiteController.OnPlayerParasite -= HandleOnPlayerParasite;
        PlayerParasiteController.OnPlayerParasiteEnd -= HandleOnPlayerParasiteEnd;

        // Exclamation Events
        PlayerHealthController.OnHealthChange -= HandleOnHealthChange;
    }
}
