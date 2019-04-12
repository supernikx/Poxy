using UnityEngine;
using System;

namespace UI
{
    public class UIMenu_TutorialPanel : UIMenu_Base
    {
        #region Delegates
        public static Action OnTutorialEnded;
        #endregion

        [Header("Tutorial Sections")]
        [SerializeField]
        private GameObject SectionA;
        [SerializeField]
        private GameObject SectionB;

        UI_ManagerBase uiManager;

        #region API
        public override void Setup(UI_ManagerBase _uiManager)
        {
            uiManager = _uiManager;
        }

        public override void Enable()
        {
            base.Enable();

            SectionA.SetActive(true);
            SectionB.SetActive(false);
        }

        //Super mega provvisorio per farlo funzionare tanto non esisterà più
        XInputDotNetPure.GamePadState joystickPrevState;
        private void Update()
        {
            switch (InputChecker.GetCurrentInputType())
            {
                case InputType.Joystick:
                    XInputDotNetPure.GamePadState joystickState = InputChecker.GetCurrentGamePadState();
                    if (joystickPrevState.Buttons.A == XInputDotNetPure.ButtonState.Released && joystickState.Buttons.A == XInputDotNetPure.ButtonState.Pressed)
                        HandleConfirmPressed();
                    joystickPrevState = joystickState;
                    break;
                case InputType.Keyboard:
                    if (Input.GetKeyDown(KeyCode.Space))
                        HandleConfirmPressed();
                    break;
            }
        }
        #endregion

        #region Handlers
        private void HandleConfirmPressed()
        {
            if (!SectionB.activeInHierarchy)
            {
                SectionB.SetActive(true);
            }
            else if (SectionA.activeInHierarchy && SectionB.activeInHierarchy)
            {
                uiManager.ToggleMenu(MenuType.None);
                if (OnTutorialEnded != null)
                    OnTutorialEnded();
            }
        }
        #endregion
    }
}

