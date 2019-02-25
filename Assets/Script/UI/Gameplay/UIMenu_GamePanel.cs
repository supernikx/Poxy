using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIMenu_GamePanel : UIMenu_Base
    {
        [Header("Reference")]
        [SerializeField]
        private Slider healthBar;
        [SerializeField]
        private Slider toleranceBar;

        private UI_ManagerBase uiManager;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;

            PlayerHealthController.OnHealthChange += OnHealthChange;
            EnemyToleranceController.OnToleranceChange += OnToleranceChange;

            EnableHealthBar(true);
            EnableToleranceBar(false);
        }

        public override void Enable()
        {
            base.Enable();

            PlayerHealthController.OnHealthChange += OnHealthChange;
            EnemyToleranceController.OnToleranceChange += OnToleranceChange;
        }
        #region PlayerHealth
        /// <summary>
        /// Funzione che si occupa dell'evento PlayerHealthController.OnHealthChange
        /// </summary>
        /// <param name="health"></param>
        private void OnHealthChange(float health)
        {
            healthBar.value = health;
        }

        /// <summary>
        /// Funzione che attiva/disattiva la barra della vita del player
        /// </summary>
        /// <param name="_enable"></param>
        public void EnableHealthBar(bool _enable)
        {
            healthBar.gameObject.SetActive(_enable);
        }
        #endregion

        #region EnemyTolerance
        /// <summary>
        /// Funzione che si occupa dell'evento EnemyToleranceController.OnToleranceChange
        /// </summary>
        /// <param name="health"></param>
        private void OnToleranceChange(float tolerance)
        {
            toleranceBar.value = tolerance;
        }

        /// <summary>
        /// Funzione che imposta il valore massimo della tolerance bar
        /// </summary>
        /// <param name="_maxTolerance"></param>
        /// <param name="_startValue"></param>
        public void SetMaxToleranceValue(float _maxTolerance)
        {
            toleranceBar.maxValue = _maxTolerance;
        }

        /// <summary>
        /// Funzione che attiva/disattiva la barra di tolleranza del nemico
        /// </summary>
        /// <param name="_enable"></param>
        public void EnableToleranceBar(bool _enable)
        {
            toleranceBar.gameObject.SetActive(_enable);
        }
        #endregion

        private void OnDisable()
        {
            PlayerHealthController.OnHealthChange -= OnHealthChange;
            EnemyToleranceController.OnToleranceChange -= OnToleranceChange;
        }

    }
}
