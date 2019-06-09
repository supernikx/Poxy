using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSoundController : SoundControllerBase
{
    [Header("General Settings")]
    [SerializeField]
    private AudioClipStruct clip;

    public void PlayClip()
    {
        PlayAudioClip(clip);
    }
}
