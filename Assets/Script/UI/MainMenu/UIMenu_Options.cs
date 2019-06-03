using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIMenu_Options : UIMenu_Base
    {
        [Header("Sound Settings")]
        [SerializeField]
        private Slider musicSlider;
        [SerializeField]
        private Slider sfxSlider;

        [Header("Screen Settings")]
        [SerializeField]
        private TMP_Dropdown resolutionDropdown;
        [SerializeField]
        private TMP_Dropdown qualityDropdown;
        [SerializeField]
        private Toggle fullScreenToggle;

        [Header("User Settings")]
        [SerializeField]
        private TMP_InputField nameInputField;

        OptionsManager optionsMng;
        //Resolution
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex;
        //Quality
        List<string> qualityOptions = new List<string>();
        int currentQualityIndex;
        //Sound
        float musicVolume;
        float sfxVolume;
        //Fullscreen
        bool fullScreen;
        //Username
        string userName;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            base.Setup(_uiManager);
            optionsMng = GameManager.instance.GetOptionsManager();

            if (resolutionDropdown != null)
                LoadResolutions(optionsMng.GetResolutions());

            if (qualityDropdown != null)
                LoadQuality(optionsMng.GetQuality());
        }

        public override void Enable()
        {
            base.Enable();
            LoadOptionsSettings();
        }

        /// <summary>
        /// Funzione che carica i settings dall'options manager
        /// </summary>
        private void LoadOptionsSettings()
        {
            //Reimposto la risoluzione
            if (resolutionDropdown != null)
            {
                currentResolutionIndex = optionsMng.GetCurrentResolutionIndex();
                resolutionDropdown.value = currentResolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }

            //Reimposto la qualità
            if (qualityDropdown != null)
            {
                currentQualityIndex = optionsMng.GetCurrentQualityIndex();
                qualityDropdown.value = currentQualityIndex;
                qualityDropdown.RefreshShownValue();
            }

            //Fullscreen
            if (fullScreenToggle != null)
            {
                fullScreen = optionsMng.IsFullScreen();
                fullScreenToggle.isOn = fullScreen;
            }

            //Volume
            if (musicSlider != null)
            {
                musicVolume = optionsMng.GetMusicVolume();
                musicSlider.value = musicVolume;
            }
            if (sfxSlider != null)
            {
                sfxVolume = optionsMng.GetSFXVolume();
                sfxSlider.value = sfxVolume;
            }

            //User
            if (nameInputField != null)
            {
                userName = optionsMng.GetUserName();
                nameInputField.text = userName;
            }
        }

        /// <summary>
        /// Funzione che imposta le risoluzioni presenti nella lista all'interno del dropdown menu e seleziona quella attuale
        /// </summary>
        private void LoadResolutions(List<Resolutions> _resolutions)
        {
            if (_resolutions.Count > 0)
            {
                resolutionDropdown.ClearOptions();
                foreach (Resolutions r in _resolutions)
                {
                    resolutionOptions.Add(r.width + " x " + r.height);
                }
                resolutionDropdown.AddOptions(resolutionOptions);
            }
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Funzione che imposta le qualità presenti nella lista all'interno del dropdown menu e seleziona quella attuale
        /// </summary>
        private void LoadQuality(List<string> _quality)
        {
            if (_quality.Count > 0)
            {
                qualityDropdown.ClearOptions();
                foreach (string q in _quality)
                {
                    qualityOptions.Add(q);
                }
                qualityDropdown.AddOptions(qualityOptions);
            }
            qualityDropdown.value = currentQualityIndex;
            qualityDropdown.RefreshShownValue();
        }

        #region Buttons
        /// <summary>
        /// Funzione che imposta la risoluzione passandogli l'index della lista come parametro
        /// </summary>
        /// <param name="_resolutionIndex"></param>
        public void SetResolution(int _resolutionIndex)
        {
            currentResolutionIndex = _resolutionIndex;
            optionsMng.SetResolution(_resolutionIndex);
        }

        /// <summary>
        /// Funzione che imposta la qualità passandogli l'index della qualità come parametro
        /// </summary>
        /// <param name="_qualityIndex"></param>
        public void SetQuality(int _qualityIndex)
        {
            currentQualityIndex = _qualityIndex;
            optionsMng.SetQuality(_qualityIndex);
        }

        /// <summary>
        /// Funzione che attiva/disattiva il fullscreen
        /// </summary>
        /// <param name="fullscreen"></param>
        public void SetFullScreen(bool _fullscreen)
        {
            fullScreen = _fullscreen;
            optionsMng.SetFullScreen(fullScreen);
        }

        /// <summary>
        /// Funzione che imposta il volume della music
        /// </summary>
        /// <param name="_volume"></param>
        public void SetMusicVolume(float _volume)
        {
            musicVolume = _volume;
            optionsMng.SetMusicVolume(_volume);
        }

        /// <summary>
        /// Funzione che imposta il volume degli SFX
        /// </summary>
        /// <param name="_volume"></param>
        public void SetSFXVolume(float _volume)
        {
            sfxVolume = _volume;
            optionsMng.SetSFXVolume(_volume);
        }

        /// <summary>
        /// Funzione che imposta l'username
        /// </summary>
        /// <param name="_userName"></param>
        public void SetUserName(string _userName)
        {
            userName = _userName;
            optionsMng.SetUserName(_userName);
        }

        /// <summary>
        /// Funzione che imposta i settings di default
        /// </summary>
        public void DefaultSettings()
        {
            optionsMng.SetDefaultValues();
            LoadOptionsSettings();
        }

        /// <summary>
        /// Funzione che gestisce il pulsante back del main menu
        /// </summary>
        public void BackMenuButton()
        {
            uiManager.ToggleMenu(MenuType.MainMenu);
        }

        /// <summary>
        /// Funzione che gestisce il pulsante back del menù di pausa
        /// </summary>
        public void BackPauseButton()
        {
            uiManager.ToggleMenu(MenuType.Pause);
        }
        #endregion

        public override void Disable()
        {
            base.Disable();
            optionsMng.SaveSettings();
        }
    }
}
