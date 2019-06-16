using UnityEngine;
using System.Collections;
using System;
using TMPro;

namespace UI
{
    public class UIMenu_CountdownPanel : UIMenu_Base
    {
        #region Delegates
        public Action OnCountdownEnd;
        #endregion

        [Header("Countdown Options")]
        [SerializeField]
        private int countFrom = 3;
        [SerializeField]
        private TextMeshProUGUI countdownText;

        private float timer;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            base.Setup(_uiManager);
            countdownText.SetText("");
        }

        public override void Enable()
        {
            base.Enable();
            StartCoroutine(CTimer());
        }

        #region Coroutines
        private IEnumerator CTimer()
        {
            timer = countFrom;

            while (timer >= 0)
            {
                if ((int)timer > 0)
                    countdownText.SetText("{0}", timer);
                else
                    countdownText.SetText("GO!!");
                timer -= Time.deltaTime;
                yield return null;
            }
            
            if (OnCountdownEnd != null)
                OnCountdownEnd();
        }
        #endregion
    } 
}
