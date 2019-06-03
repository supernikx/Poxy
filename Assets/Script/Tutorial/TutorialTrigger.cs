using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    #region Action
    public static Action<TutorialTrigger> OnTutorialBehaviourTriggered;
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
    private string joystickTextToShow;
    [SerializeField]
    private string keyboardTextToShow;
    [SerializeField]
    private Sprite artToShow;

    private TutorialWallTrigger wallTrigger;

    #region API
    public void Init()
    {
        wallTrigger = GetComponentInChildren<TutorialWallTrigger>(true);
        if (wallTrigger != null)
            wallTrigger.Enable(true);

        tutorialSpriteRenderer.sprite = artToShow;
        tutorialSpriteRenderer.gameObject.SetActive(false);
    }

    #region Getter
    public string GetTextToShow(InputType _type)
    {
        switch (_type)
        {
            case InputType.Joystick:
                return joystickTextToShow;
            case InputType.Keyboard:
                return keyboardTextToShow;
        }
        return null;
    }

    public TriggerBehaviours GetBehvaiourToTrigger()
    {
        return triggerBehaviours;
    }
    #endregion
    #endregion

    private void HandleTutorialWallTriggered()
    {
        wallTrigger.Enable(false);
        if (OnTutorialBehaviourTriggered != null)
            OnTutorialBehaviourTriggered(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {

            tutorialSpriteRenderer.gameObject.SetActive(true);

            if (wallTrigger != null)
                wallTrigger.OnWallTriggered += HandleTutorialWallTriggered;

            if (OnTutorialTriggerEnter != null)
                OnTutorialTriggerEnter(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            tutorialSpriteRenderer.gameObject.SetActive(false);

            if (wallTrigger != null)
                wallTrigger.OnWallTriggered -= HandleTutorialWallTriggered;

            if (OnTutorialTriggerExit != null)
                OnTutorialTriggerExit(this);
        }
    }

    private void OnDisable()
    {
        if (wallTrigger != null)
            wallTrigger.OnWallTriggered -= HandleTutorialWallTriggered;
    }

    public enum TriggerBehaviours
    {
        None,
        HealthBar,
        TolleranceBar,
    }
}
