using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private GameObject triggersContainer;
    [SerializeField]
    private TextMeshProUGUI tutorialText;

    #region API
    public void Init()
    {
        TutorialTrigger.OnTutorialTriggerEnter += HandleTutorialTriggerEnter;
        TutorialTrigger.OnTutorialTriggerExit += HandleTutorialTriggerExit;

        foreach (TutorialTrigger trigger in triggersContainer.GetComponentsInChildren<TutorialTrigger>())
        {
            trigger.Init();
        }

        tutorialText.gameObject.SetActive(false);
    }
    #endregion

    private void HandleTutorialTriggerEnter(TutorialTrigger _triggerTriggered)
    {
        tutorialText.text = _triggerTriggered.GetTextToShow();
        tutorialText.gameObject.SetActive(true);
    }

    private void HandleTutorialTriggerExit(TutorialTrigger _triggerExit)
    {
        tutorialText.gameObject.SetActive(false);

        switch (_triggerExit.GetBehvaiourToTrigger())
        {
            case TutorialTrigger.TriggerBehaviours.HealthBar:
                Debug.Log("Vita attivata");
                break;
            case TutorialTrigger.TriggerBehaviours.TolleranceBar:
                Debug.Log("Tollerance attivata");
                break;
        }
    }

    private void OnDisable()
    {
        TutorialTrigger.OnTutorialTriggerEnter -= HandleTutorialTriggerEnter;
        TutorialTrigger.OnTutorialTriggerExit -= HandleTutorialTriggerExit;
    }
}
