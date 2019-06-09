using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : SoundControllerBase
{
    [Header("Background Music Settings")]
    [SerializeField]
    private AudioClipStruct music;

    public override void Init()
    {
        base.Init();
        PlayAudioClip(music);
    }
}
