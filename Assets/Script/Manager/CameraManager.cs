using UnityEngine;
using System;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    #region Actions
    public static Action<bool> OnZoomOutTriggered;
    #endregion

    [Header("Camera Target")]
    [SerializeField]
    private CinemachineVirtualCamera playerVirtualCamera;
    [SerializeField]
    private CinemachineVirtualCamera playerVirtualCameraZoomOut;
    [SerializeField]
    private CinemachineVirtualCamera platformVirtualCamera;

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
        noise = playerVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        PlayerHealthController.OnHealthChange += HandleOnHealthChange;
        LevelManager.OnPlayerDeath += HandleOnPlayerDeath;
        OnZoomOutTriggered += HandleZoomOutTriggered;
    }

    public void SetPlatformCamera(Transform _platformTarget)
    {
        platformVirtualCamera.Follow = _platformTarget;
        platformVirtualCamera.gameObject.SetActive(true);
        playerVirtualCamera.gameObject.SetActive(false);
    }

    public void SetPlayerCamera()
    {
        platformVirtualCamera.gameObject.SetActive(false);
        playerVirtualCamera.gameObject.SetActive(true);
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

    private void HandleZoomOutTriggered(bool _zoomOutEnable)
    {
        playerVirtualCameraZoomOut.gameObject.SetActive(_zoomOutEnable);

        //Noise settings
        float oldApmplitudeValue = 0;
        float oldFrequencyValue = 0;
        if (noise != null)
        {
            oldApmplitudeValue = noise.m_AmplitudeGain;
            oldFrequencyValue = noise.m_FrequencyGain;
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
        }

        if (_zoomOutEnable)
            noise = playerVirtualCameraZoomOut.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        else
            noise = playerVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = oldApmplitudeValue;
        noise.m_FrequencyGain = oldFrequencyValue;
    }


    private void HandleOnPlayerDeath()
    {
        if (playerVirtualCameraZoomOut.gameObject.activeSelf)
            playerVirtualCameraZoomOut.gameObject.SetActive(false);

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
        OnZoomOutTriggered -= HandleZoomOutTriggered;
    }
}

public enum CameraState
{
    Normal,
    Shake
}
