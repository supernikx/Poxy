using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        private UI_ManagerBase uiManager;
        private ObjectTypes activeBullet;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;
            PlayerHealthController.OnHealthChange += HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged += HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange += HandleOnToleranceChange;
            HandleOnEnemyBulletChanged(ObjectTypes.None);
            EnableHealthBar(true);
            EnableToleranceBar(false);
            activeBullet = ObjectTypes.StunBullet;
        }

        public override void Enable()
        {
            base.Enable();
            PlayerHealthController.OnHealthChange += HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged += HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange += HandleOnToleranceChange;
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

        private void OnDisable()
        {
            PlayerHealthController.OnHealthChange -= HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged -= HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange -= HandleOnToleranceChange;
        }
    }
}
