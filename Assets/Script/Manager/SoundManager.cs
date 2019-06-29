using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private AudioMixerGroup musicOutput;
    [SerializeField]
    private AudioMixerGroup sfxOutput;

    [Header("Background Settings")]
    [SerializeField]
    private BackgroundMusicController mainMenuMusic;
    [SerializeField]
    private BackgroundMusicController gameMusic;

    public void Init()
    {
        if (mainMenuMusic != null)
            mainMenuMusic.Init();
        if (gameMusic != null)
            gameMusic.Init();

        switch (SceneManager.GetActiveScene().name)
        {
            case "MainMenu":
                if (mainMenuMusic != null)
                    mainMenuMusic.Play();
                break;
            case "Level1":
            case "Tutorial":
                if (gameMusic != null)
                    gameMusic.Play();
                break;
        }
    }

    /// <summary>
    /// Funzione che stoppa la musica di game efa partire quella del main menu
    /// </summary>
    public void PlayMainMenuMnusic()
    {
        if (gameMusic != null)
            gameMusic.Stop();

        if (mainMenuMusic != null)
            mainMenuMusic.Play();
    }

    /// <summary>
    /// Funzione che stoppa la musica del main menu e fa partire quella di game
    /// </summary>
    public void PlayGameMusic()
    {
        if (mainMenuMusic != null)
            mainMenuMusic.Stop();

        if (gameMusic != null)
            gameMusic.Play();
    }

    /// <summary>
    /// Funzione che stoppa la musica
    /// </summary>
    public void StopMusic()
    {
        if (mainMenuMusic != null)
            mainMenuMusic.Stop();

        if (gameMusic != null)
            gameMusic.Stop();
    }

    /// <summary>
    /// Funzione che imposta il volume della musica con il valore passato come parametro
    /// </summary>
    /// <param name="_volume"></param>
    public void SetMusicVolume(float _volume)
    {
        audioMixer.SetFloat("MusicVolume", _volume);
    }

    /// <summary>
    /// Funzione che imposta il volume degli sfx con il valore passato come parametro
    /// </summary>
    /// <param name="_volume"></param>
    public void SetSFXVolume(float _volume)
    {
        audioMixer.SetFloat("SFXVolume", _volume);
    }

    public AudioMixerGroup GetOutputGroup(SoundOutput _output)
    {
        switch (_output)
        {
            case SoundOutput.Music:
                return musicOutput;
            case SoundOutput.SFX:
                return sfxOutput;
        }

        return null;
    }
}
