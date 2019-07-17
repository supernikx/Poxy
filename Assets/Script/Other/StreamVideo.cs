using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class StreamVideo : MonoBehaviour
{
    #region Actions
    public Action OnVideoLoad;
    public Action OnVideoStart;
    public Action OnVideoEnd;
    #endregion

    [SerializeField]
    private RawImage rawImage;
    [SerializeField]
    private VideoPlayer videoPlayer;
    [SerializeField]
    private AudioSource audioSource;

    public void LoadVideo()
    {        
        StartCoroutine(LoadVideoRoutine());
    }

    public void PlayVideo()
    {
        if (videoPlayer.isPrepared)
        {            
            videoPlayer.Play();
            audioSource.Play();
            rawImage.gameObject.SetActive(true);
            StartCoroutine(CheckDurationRoutine());

            OnVideoStart?.Invoke();
        }
    }

    private IEnumerator LoadVideoRoutine()
    {
        videoPlayer.Prepare();
        WaitForSeconds wfs = new WaitForSeconds(1f);
        while (!videoPlayer.isPrepared)
        {
            yield return wfs;
        }

        rawImage.texture = videoPlayer.texture;
        OnVideoLoad?.Invoke();
    }

    private IEnumerator CheckDurationRoutine()
    {        
        while (videoPlayer.time < (videoPlayer.length - 0.1f))
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButton("Submit"))
                break;
            yield return null;
        }

        rawImage.gameObject.SetActive(false);
        OnVideoEnd?.Invoke();
    }
}
