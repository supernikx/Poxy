using UnityEngine;
using System.Collections;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Target")]
    [SerializeField]
    private CinemachineVirtualCamera targetCamera;

    [Header("Camera Shake Options")]
    [SerializeField]
    private float amplitudeGain;
    [SerializeField]
    private float frequencyGain;
    [SerializeField]
    private float shakeActivationPercentage;

    private CameraState currentState;
    private CinemachineBasicMultiChannelPerlin noise;

    #region API
    public void Init()
    {
        currentState = CameraState.Normal;
        noise = targetCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        PlayerHealthController.OnHealthChange += HandleOnHealthChange;
        LevelManager.OnPlayerDeath += HandleOnPlayerDeath;
    }
    #endregion

    #region Handlers
    private void HandleOnHealthChange(float _health)
    {
        if (_health <= shakeActivationPercentage && currentState != CameraState.Shake)
        {
            currentState = CameraState.Shake;
            noise.m_AmplitudeGain = amplitudeGain;
            noise.m_FrequencyGain = frequencyGain;
        }
        else if (_health > shakeActivationPercentage && currentState == CameraState.Shake)
        {
            currentState = CameraState.Normal;
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
        }
    }

    private void HandleOnPlayerDeath()
    {
        if (currentState == CameraState.Shake)
        {
            currentState = CameraState.Normal;
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
        }
    }
    #endregion

    private void OnDisable()
    {
        PlayerHealthController.OnHealthChange -= HandleOnHealthChange;
        LevelManager.OnPlayerDeath -= HandleOnPlayerDeath;
    }
}

public enum CameraState
{
    Normal,
    Shake
}
