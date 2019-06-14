﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIMenu_GamePanel : UIMenu_Base
    {
        [Header("LowLife Panel")]
        [SerializeField]
        private Image lowLifePanel;
        [SerializeField]
        private float lowLifePanelLife;

        [Header("Health Bar")]
        [SerializeField]
        private GameObject healthBar;
        [SerializeField]
        private Image healthBarFill;
        [SerializeField]
        private float playerMaxHealth;

        [Header("Tolerance Bar")]
        [SerializeField]
        private GameObject toleranceBar;
        [SerializeField]
        private Image toleranceBarFill;
        private float maxTolerance;

        [Header("Character Image")]
        [SerializeField]
        private float changeCharacterLifeValue;
        [SerializeField]
        private Image characterUI;
        [SerializeField]
        private Sprite poxyCalmSprite;
        [SerializeField]
        private Sprite poxyAngrySprite;
        [SerializeField]
        private Sprite parabolicEnemySprite;
        [SerializeField]
        private Sprite stickyEnemySprite;

        [Header("Speedrun Panel")]
        [SerializeField]
        private GameObject speedrunPanel;
        [SerializeField]
        private TextMeshProUGUI timerText;
        [SerializeField]
        private Color timerDefaultColor;
        [SerializeField]
        private Color timerHoldColor;

        [Header("Token Panel")]
        [SerializeField]
        private GameObject tokenPanel;
        [SerializeField]
        private TextMeshProUGUI tokenText;
        [SerializeField]
        private float secondsOnScreen;
        private bool tokenPanelActive = false;

        private ObjectTypes activeBullet;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            HandleOnEnemyBulletChanged(ObjectTypes.None);
            EnableHealthBar(true);
            EnableToleranceBar(false);
            activeBullet = ObjectTypes.StunBullet;
            timerText.color = timerDefaultColor;
        }

        public override void Enable()
        {
            base.Enable();
            PlayerHealthController.OnHealthChange += HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged += HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange += HandleOnToleranceChange;
            SpeedrunManager.OnTimerUpdate += HandleOnTimerUpdate;
            SpeedrunManager.OnTimerHold += HandleOnTimerHold;
            TokenManager.OnTokenTaken += HandleOnTokenTaken;

            timerText.color = timerDefaultColor;
            ToggleSpeedrunPanel(LevelManager.instance.GetSpeedrunManager().GetIsActive());
            ToggleTokenPanel(false);
        }

        #region Character
        private void HandleOnEnemyBulletChanged(ObjectTypes _bullet)
        {
            activeBullet = _bullet;
            switch (activeBullet)
            {
                case ObjectTypes.ParabolicBullet:
                    characterUI.sprite = parabolicEnemySprite;
                    break;
                case ObjectTypes.StickyBullet:
                    characterUI.sprite = stickyEnemySprite;
                    break;
                case ObjectTypes.StunBullet:
                    if (healthBarFill.fillAmount > changeCharacterLifeValue)
                        characterUI.sprite = poxyCalmSprite;
                    else
                        characterUI.sprite = poxyAngrySprite;
                    break;
            }
        }
        #endregion

        #region PlayerHealth
        /// <summary>
        /// Funzione che si occupa dell'evento PlayerHealthController.OnHealthChange
        /// </summary>
        /// <param name="health"></param>
        private void HandleOnHealthChange(float health)
        {
            float healthPercentage = health / playerMaxHealth;
            healthBarFill.fillAmount = healthPercentage;
            if (health <= lowLifePanelLife)
            {
                if (!lowLifePanel.gameObject.activeSelf)
                    EnableLowLifePanel(true);
                Color newAlpha = new Color(1f, 1f, 1f, 1f - health / lowLifePanelLife);
                lowLifePanel.color = newAlpha;
            }
            else if (lowLifePanel.gameObject.activeSelf)
                EnableLowLifePanel(false);

            if (activeBullet == ObjectTypes.StunBullet)
            {
                if (health > changeCharacterLifeValue)
                    characterUI.sprite = poxyCalmSprite;
                else
                    characterUI.sprite = poxyAngrySprite;
            }
        }

        /// <summary>
        /// Funzione che attiva/disattiva la barra della vita del player
        /// </summary>
        /// <param name="_enable"></param>
        public void EnableHealthBar(bool _enable)
        {
            healthBar.SetActive(_enable);
        }

        /// <summary>
        /// Funzione che attiva/disattiva il pannello lowlife
        /// </summary>
        /// <param name="_enable"></param>
        public void EnableLowLifePanel(bool _enable)
        {
            lowLifePanel.gameObject.SetActive(_enable);
        }
        #endregion

        #region EnemyTolerance
        /// <summary>
        /// Funzione che si occupa dell'evento EnemyToleranceController.OnToleranceChange
        /// </summary>
        /// <param name="health"></param>
        private void HandleOnToleranceChange(float tolerance)
        {
            float tolerancePercentage = tolerance / maxTolerance;
            toleranceBarFill.fillAmount = 1 - tolerancePercentage;
        }

        /// <summary>
        /// Funzione che imposta il valore massimo della tolerance bar
        /// </summary>
        /// <param name="_maxTolerance"></param>
        /// <param name="_startValue"></param>
        public void SetMaxToleranceValue(float _maxTolerance)
        {
            maxTolerance = _maxTolerance;
        }

        /// <summary>
        /// Funzione che attiva/disattiva la barra di tolleranza del nemico
        /// </summary>
        /// <param name="_enable"></param>
        public void EnableToleranceBar(bool _enable)
        {
            toleranceBar.SetActive(_enable);
        }
        #endregion

        #region Speedrun
        private void ToggleSpeedrunPanel(bool _val)
        {
            speedrunPanel.SetActive(_val);
        }

        private void HandleOnTimerUpdate(float _timer)
        {
            timerText.SetText("{0:2}", _timer);
        }

        private void HandleOnTimerHold(bool _val)
        {
            if (_val)
                timerText.color = timerHoldColor;
            else
                timerText.color = timerDefaultColor;
        }
        #endregion

        #region Token
        private void HandleOnTokenTaken(int _val)
        {
            tokenText.SetText("Tokens: {0}", _val);

            if (!tokenPanelActive)
            {
                StartCoroutine(CTokenPanelActive());
                ToggleTokenPanel(true);
            }
            else
                tokenPanelTimer = 0;
        }

        float tokenPanelTimer = 0;
        private IEnumerator CTokenPanelActive()
        {
            while (tokenPanelTimer <= secondsOnScreen)
            {
                tokenPanelTimer += Time.deltaTime;
                yield return null;
            }

            tokenPanelTimer = 0;
            ToggleTokenPanel(false);
        }

        private void ToggleTokenPanel(bool _val)
        {
            tokenPanelActive = _val;
            tokenPanel.SetActive(_val);
        }
        #endregion

        private void OnDisable()
        {
            PlayerHealthController.OnHealthChange -= HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged -= HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange -= HandleOnToleranceChange;
            SpeedrunManager.OnTimerUpdate -= HandleOnTimerUpdate;
            SpeedrunManager.OnTimerHold -= HandleOnTimerHold;
            TokenManager.OnTokenTaken -= HandleOnTokenTaken;
        }
    }
}
