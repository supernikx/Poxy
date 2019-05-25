using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Reference Settings")]
    [SerializeField]
    private TextMeshProUGUI tutorialText;
    [SerializeField]
    private Image tutorialImage;

    [Header("Trigger Settings")]
    [SerializeField]
    private TriggerBehaviours triggerBehaviours;
    [SerializeField]
    private string textToShow;
    [SerializeField]
    private Sprite artToShow;

    public void Init()
    {
        tutorialText.text = textToShow;
        tutorialImage.sprite = artToShow;

        tutorialText.gameObject.SetActive(false);
        tutorialImage.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            tutorialText.gameObject.SetActive(true);
            tutorialImage.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("PlayerImmunity"))
        {
            tutorialText.gameObject.SetActive(false);
            tutorialImage.gameObject.SetActive(false);
        }
    }


    public enum TriggerBehaviours
    {
        None,
        HealthBar,
        TolleranceBar,
    }
}
