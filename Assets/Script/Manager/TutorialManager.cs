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

    EnemyManager enemyMng;
    Player player;
    List<TutorialTrigger> triggers = new List<TutorialTrigger>();

    #region API
    public void Init(EnemyManager _enemyMng, Player _player)
    {
        LevelManager.OnPlayerDeath += HandlePlayerDeath;
        TutorialTrigger.OnTutorialBehaviourTriggered += HandleTutorialBehaviourTriggered;
        TutorialTrigger.OnTutorialTriggerEnter += HandleTutorialTriggerEnter;
        TutorialTrigger.OnTutorialTriggerExit += HandleTutorialTriggerExit;

        enemyMng = _enemyMng;
        player = _player;
        triggers = triggersContainer.GetComponentsInChildren<TutorialTrigger>().ToList();

        TriggersSetup();
        LevelSettings();

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

    private void LevelSettings()
    {
        player.GetHealthController().SetCanLoseHealthDefaultBehaviour(false);
        enemyMng.SetCanAddTolleranceDefaultBehaviour(false);
    }

    #region Handler
    private void HandleTutorialBehaviourTriggered(TutorialTrigger _trigger)
    {
        switch (_trigger.GetBehvaiourToTrigger())
        {
            case TutorialTrigger.TriggerBehaviours.HealthBar:
                player.GetHealthController().SetCanLoseHealthDefaultBehaviour(true);
                break;
            case TutorialTrigger.TriggerBehaviours.TolleranceBar:
                enemyMng.SetCanAddTolleranceDefaultBehaviour(true);
                break;
        }
    }

    private void HandleTutorialTriggerEnter(TutorialTrigger _triggerTriggered)
    {
        enemyMng.SetCanAddTollerance(false);
        player.GetHealthController().SetCanLoseHealth(false);
        tutorialText.text = _triggerTriggered.GetTextToShow();
        tutorialText.gameObject.SetActive(true);
    }

    private void HandleTutorialTriggerExit(TutorialTrigger _triggerExit)
    {
        tutorialText.gameObject.SetActive(false);
        player.GetHealthController().SetCanLoseHealth(true);
        enemyMng.SetCanAddTollerance(true);
    }

    private void HandlePlayerDeath()
    {
        TriggersSetup();
        LevelSettings();

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
