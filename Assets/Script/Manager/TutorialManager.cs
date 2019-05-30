using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private GameObject triggersContainer;
    [SerializeField]
    private TextMeshProUGUI tutorialText;

    Player player;
    List<TutorialTrigger> triggers = new List<TutorialTrigger>();

    #region API
    public void Init(Player _player)
    {
        LevelManager.OnPlayerDeath += HandlePlayerDeath;
        TutorialTrigger.OnTutorialBehaviourTriggered += HandleTutorialBehaviourTriggered;
        TutorialTrigger.OnTutorialTriggerEnter += HandleTutorialTriggerEnter;
        TutorialTrigger.OnTutorialTriggerExit += HandleTutorialTriggerExit;

        player = _player;
        triggers = triggersContainer.GetComponentsInChildren<TutorialTrigger>().ToList();

        TriggersSetup();
        PlayerSettings();

        tutorialText.gameObject.SetActive(false);
    }

    #endregion

    private void TriggersSetup()
    {
        foreach (TutorialTrigger trigger in triggers)
        {
            trigger.Init();
        }
    }

    private void PlayerSettings()
    {
        player.GetHealthController().SetCanLoseHealthDefaultBehaviour(false);
    }

    #region Handler
    private void HandleTutorialBehaviourTriggered(TutorialTrigger _trigger)
    {
        switch (_trigger.GetBehvaiourToTrigger())
        {
            case TutorialTrigger.TriggerBehaviours.HealthBar:
                player.GetHealthController().SetCanLoseHealthDefaultBehaviour(true);
                break;
        }
    }

    private void HandleTutorialTriggerEnter(TutorialTrigger _triggerTriggered)
    {
        player.GetHealthController().SetCanLoseHealth(false);
        tutorialText.text = _triggerTriggered.GetTextToShow();
        tutorialText.gameObject.SetActive(true);
    }

    private void HandleTutorialTriggerExit(TutorialTrigger _triggerExit)
    {
        tutorialText.gameObject.SetActive(false);
        player.GetHealthController().SetCanLoseHealth(true);
    }

    private void HandlePlayerDeath()
    {
        TriggersSetup();
        PlayerSettings();

        tutorialText.gameObject.SetActive(false);
    }
    #endregion

    private void OnDisable()
    {
        LevelManager.OnPlayerDeath -= HandlePlayerDeath;
        TutorialTrigger.OnTutorialBehaviourTriggered -= HandleTutorialBehaviourTriggered;
        TutorialTrigger.OnTutorialTriggerEnter -= HandleTutorialTriggerEnter;
        TutorialTrigger.OnTutorialTriggerExit -= HandleTutorialTriggerExit;
    }
}
