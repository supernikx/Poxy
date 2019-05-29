using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    #region Action
    public static Action<TutorialTrigger> OnTutorialTriggerEnter;
    public static Action<TutorialTrigger> OnTutorialTriggerExit;
    #endregion

    [Header("Reference Settings")]
    [SerializeField]
    private SpriteRenderer tutorialSpriteRenderer;

    [Header("Trigger Settings")]
    [SerializeField]
    private TriggerBehaviours triggerBehaviours;
    [SerializeField]
    private string textToShow;
    [SerializeField]
    private Sprite artToShow;

    #region API
    public void Init()
    {
        tutorialSpriteRenderer.sprite = artToShow;
        tutorialSpriteRenderer.gameObject.SetActive(false);
    }

    #region Getter
    public string GetTextToShow()
    {
        return textToShow;
    }

    public TriggerBehaviours GetBehvaiourToTrigger()
    {
        return triggerBehaviours;
    }
    #endregion
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            tutorialSpriteRenderer.gameObject.SetActive(true);
            if (OnTutorialTriggerEnter != null)
                OnTutorialTriggerEnter(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            tutorialSpriteRenderer.gameObject.SetActive(false);
            if (OnTutorialTriggerExit != null)
                OnTutorialTriggerExit(this);
        }
    }


    public enum TriggerBehaviours
    {
        None,
        HealthBar,
        TolleranceBar,
    }
}
