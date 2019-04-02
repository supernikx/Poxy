using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenu_GamePanel : UIMenu_Base
    {
        [Header("Life UI")]
        [SerializeField]
        private Image hearth1Image;
        [SerializeField]
        private Image hearth2Image;
        [SerializeField]
        private Image hearth3Image;
        [Header("Health Bar")]
        [SerializeField]
        private float playerMaxHealth;
        [SerializeField]
        private float circleHealthAmmount;
        private float circleFillBaseAmmount = 0.481f;
        [SerializeField]
        private Image healthBarCircleFill;
        [SerializeField]
        private Image healthBarHorizontalFill;
        [Header("Tolerance Bar")]
        [SerializeField]
        private Image toleranceBarFill;
        [SerializeField]
        private Image toleranceBarContorno;
        private float maxTolerance;
        private float maxToleranceBarValue = 0.987f;
        private float minToleranceBarValue = 0.1f;
        private float toleranceOffset;
        [Header("Bullet Image")]
        [SerializeField]
        private Image damageBulletSlot;
        [SerializeField]
        private Image damageBulletContorno;
        [SerializeField]
        private Sprite parabolicBulletSprite;
        [SerializeField]
        private Sprite stickyBulletSprite;

        private UI_ManagerBase uiManager;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;

            PlayerLivesController.OnLivesChange += HandleLivesChange;
            PlayerHealthController.OnHealthChange += HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged += HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange += HandleOnToleranceChange;

            toleranceOffset = maxToleranceBarValue - minToleranceBarValue;
            HandleOnEnemyBulletChanged(ObjectTypes.None);
            EnableHealthBar(true);
            EnableToleranceBar(false);
        }

        public override void Enable()
        {
            base.Enable();

            PlayerLivesController.OnLivesChange += HandleLivesChange;
            PlayerHealthController.OnHealthChange += HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged += HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange += HandleOnToleranceChange;
        }

        #region Bullet
        private void HandleOnEnemyBulletChanged(ObjectTypes _bullet)
        {
            Color newImageColor;
            switch (_bullet)
            {
                case ObjectTypes.ParabolicBullet:
                    damageBulletSlot.sprite = parabolicBulletSprite;
                    newImageColor = new Color(damageBulletSlot.color.r, damageBulletSlot.color.g, damageBulletSlot.color.b, 1f);
                    damageBulletSlot.color = newImageColor;
                    newImageColor = new Color(damageBulletContorno.color.r, damageBulletContorno.color.g, damageBulletContorno.color.b, 1f);
                    damageBulletContorno.color = newImageColor;
                    break;
                case ObjectTypes.StickyBullet:
                    damageBulletSlot.sprite = stickyBulletSprite;
                    newImageColor = new Color(damageBulletSlot.color.r, damageBulletSlot.color.g, damageBulletSlot.color.b, 1f);
                    damageBulletSlot.color = newImageColor;
                    newImageColor = new Color(damageBulletContorno.color.r, damageBulletContorno.color.g, damageBulletContorno.color.b, 1f);
                    damageBulletContorno.color = newImageColor;
                    break;
                case ObjectTypes.None:
                default:
                    damageBulletSlot.sprite = null;
                    newImageColor = new Color(damageBulletSlot.color.r, damageBulletSlot.color.g, damageBulletSlot.color.b, 0f);
                    damageBulletSlot.color = newImageColor;
                    newImageColor = new Color(damageBulletContorno.color.r, damageBulletContorno.color.g, damageBulletContorno.color.b, 0.15f);
                    damageBulletContorno.color = newImageColor;
                    break;
            }
        }
        #endregion

        #region PlayerLives
        private void HandleLivesChange(int lives)
        {
            switch (lives)
            {
                case 0:
                    hearth1Image.gameObject.SetActive(false);
                    hearth2Image.gameObject.SetActive(false);
                    hearth3Image.gameObject.SetActive(false);
                    break;
                case 1:
                    hearth1Image.gameObject.SetActive(true);
                    hearth2Image.gameObject.SetActive(false);
                    hearth3Image.gameObject.SetActive(false);
                    break;
                case 2:
                    hearth1Image.gameObject.SetActive(true);
                    hearth2Image.gameObject.SetActive(true);
                    hearth3Image.gameObject.SetActive(false);
                    break;
                case 3:
                default:
                    hearth1Image.gameObject.SetActive(true);
                    hearth2Image.gameObject.SetActive(true);
                    hearth3Image.gameObject.SetActive(true);
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

            if (health > circleHealthAmmount)
            {
                float barPercentageFill = (health - circleHealthAmmount) / (playerMaxHealth - circleHealthAmmount);
                healthBarCircleFill.fillAmount = circleFillBaseAmmount;
                healthBarHorizontalFill.fillAmount = barPercentageFill;
            }
            else
            {
                float circlePercentageFill = healthPercentage / (circleHealthAmmount / 100f);
                circlePercentageFill *= circleFillBaseAmmount;
                healthBarHorizontalFill.fillAmount = 0f;
                healthBarCircleFill.fillAmount = circlePercentageFill;
            }
        }

        /// <summary>
        /// Funzione che attiva/disattiva la barra della vita del player
        /// </summary>
        /// <param name="_enable"></param>
        public void EnableHealthBar(bool _enable)
        {
            healthBarCircleFill.gameObject.SetActive(_enable);
            healthBarHorizontalFill.gameObject.SetActive(_enable);
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
            toleranceBarFill.fillAmount = minToleranceBarValue + (toleranceOffset * tolerancePercentage);
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
            toleranceBarFill.gameObject.SetActive(_enable);

            if (_enable)
            {
                Color newImageColor = new Color(toleranceBarContorno.color.r, toleranceBarContorno.color.g, toleranceBarContorno.color.b, 1f);
                toleranceBarContorno.color = newImageColor;
            }
            else
            {
                Color newImageColor = new Color(toleranceBarContorno.color.r, toleranceBarContorno.color.g, toleranceBarContorno.color.b, 0.15f);
                toleranceBarContorno.color = newImageColor;
            }
        }
        #endregion

        private void OnDisable()
        {
            PlayerLivesController.OnLivesChange -= HandleLivesChange;
            PlayerHealthController.OnHealthChange -= HandleOnHealthChange;
            PlayerShotController.OnEnemyBulletChanged -= HandleOnEnemyBulletChanged;
            EnemyToleranceController.OnToleranceChange -= HandleOnToleranceChange;
        }
    }
}
