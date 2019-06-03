using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    [Header("Sound Settings")]
    [SerializeField]
    private AudioMixer audioMixer;

    [Header("Screen Settings")]
    [SerializeField]
    private List<Resolutions> resolutions = new List<Resolutions>();
    [SerializeField]
    private List<string> quality = new List<string>();

    float musicVolume;
    float sfxVolume;
    int currentResolutionIndex;
    int currentQualityIndex;
    bool fullScreen;
    string userName;

    public void Init()
    {
        LoadSettings();
    }

    /// <summary>
    /// Funzione che salva i settings
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("Resolution", currentResolutionIndex);
        PlayerPrefs.SetInt("Quality", currentQualityIndex);
        if (Screen.fullScreen)
            PlayerPrefs.SetInt("FullScreen", 1);
        else
            PlayerPrefs.SetInt("FullScreen", 0);
        PlayerPrefs.SetString("UserName", userName);
    }

    #region Getter
    /// <summary>
    /// Funzione che ritorna le risoluzioni
    /// </summary>
    /// <returns></returns>
    public List<Resolutions> GetResolutions()
    {
        return resolutions;
    }

    /// <summary>
    /// Funzione che ritorna le qualità
    /// </summary>
    /// <returns></returns>
    public List<string> GetQuality()
    {
        return quality;
    }

    /// <summary>
    /// Funzione che ritorna l'index della risoluzione attuale
    /// </summary>
    /// <returns></returns>
    public int GetCurrentResolutionIndex()
    {
        return currentResolutionIndex;
    }

    /// <summary>
    /// Funzione che ritorna l'index della qualità attuale
    /// </summary>
    /// <returns></returns>
    public int GetCurrentQualityIndex()
    {
        return currentQualityIndex;
    }

    /// <summary>
    /// Funzione che ritorna se è full screen
    /// </summary>
    /// <returns></returns>
    public bool IsFullScreen()
    {
        return fullScreen;
    }

    /// <summary>
    /// Funzione che ritorna il volume della musica
    /// </summary>
    /// <returns></returns>
    public float GetMusicVolume()
    {
        return musicVolume;
    }

    /// <summary>
    /// Funzione che ritorna il volume degli sfx
    /// </summary>
    /// <returns></returns>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    /// <summary>
    /// Funzione che ritorna l'username
    /// </summary>
    /// <returns></returns>
    public string GetUserName()
    {
        return userName;
    }
    #endregion

    #region Setter
    /// <summary>
    /// Funzione che imposta la risoluzione passandogli l'index della lista come parametro
    /// </summary>
    /// <param name="_resolutionIndex"></param>
    public void SetResolution(int _resolutionIndex)
    {
        currentResolutionIndex = _resolutionIndex;
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", currentResolutionIndex);
    }

    /// <summary>
    /// Funzione che imposta la qualità passandogli l'index della qualità come parametro
    /// </summary>
    /// <param name="_qualityIndex"></param>
    public void SetQuality(int _qualityIndex)
    {
        currentQualityIndex = _qualityIndex;
        QualitySettings.SetQualityLevel(currentQualityIndex);
        PlayerPrefs.SetInt("Quality", currentQualityIndex);
    }

    /// <summary>
    /// Funzione che attiva/disattiva il fullscreen
    /// </summary>
    /// <param name="fullscreen"></param>
    public void SetFullScreen(bool _fullscreen)
    {
        fullScreen = _fullscreen;
        Screen.fullScreen = fullScreen;
        if (fullScreen)
            PlayerPrefs.SetInt("FullScreen", 1);
        else
            PlayerPrefs.SetInt("FullScreen", 0);
    }

    /// <summary>
    /// funzione che imposta il volume della musica
    /// </summary>
    /// <param name="volume"></param>
    public void SetMusicVolume(float _volume)
    {
        musicVolume = _volume;
        //audioMixer.SetFloat("musicvolume", musicVolume);
        PlayerPrefs.SetFloat("musicvolume", musicVolume);
    }

    /// <summary>
    /// Funzione che imposta il volume degli SFX
    /// </summary>
    /// <param name="volume"></param>
    public void SetSFXVolume(float _volume)
    {
        sfxVolume = _volume;
        //audioMixer.SetFloat("SFXVolume", effectsVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    /// <summary>
    /// Funzione che imposta l'username
    /// </summary>
    /// <param name="_userName"></param>
    public void SetUserName(string _userName)
    {
        userName = _userName;
        PlayerPrefs.SetString("UserName", userName);
    }

    /// <summary>
    /// Funzione che reimposta i settings ai valori di default
    /// </summary>
    public void SetDefaultValues()
    {
        SetFullScreen(true);
        SetResolution(0);
        SetQuality(2);
        SetMusicVolume(0);
        SetSFXVolume(0);
        SaveSettings();
    }
    #endregion

    /// <summary>
    /// Funzione che carica i settaggi all'avvio del gioco
    /// </summary>
    private void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0);
        //audioMixer.SetFloat("MusicVolume", musicVolume);
        //audioMixer.SetFloat("SFXVolume", sfxVolume);
        if (PlayerPrefs.GetInt("FullScreen", 1) == 1)
        {
            Screen.fullScreen = true;
            fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
            fullScreen = false;
        }
        currentResolutionIndex = PlayerPrefs.GetInt("Resolution", 0);
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, Screen.fullScreen);
        currentQualityIndex = PlayerPrefs.GetInt("Quality", 2);
        QualitySettings.SetQualityLevel(currentQualityIndex);

        userName = PlayerPrefs.GetString("UserName", "");
        if (userName == "")
        {
            userName = "Guest" + Random.Range(0, int.MaxValue);
            PlayerPrefs.SetString("UserName", userName);
        }
    }
}

[System.Serializable]
public class Resolutions
{
    public int width;
    public int height;
}
