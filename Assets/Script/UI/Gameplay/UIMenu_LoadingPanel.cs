using UnityEngine;
using TMPro;

namespace UI
{
    public class UIMenu_LoadingPanel : UIMenu_Base
    {
        [SerializeField]
        private GameObject livesPanel;
        [SerializeField]
        private TextMeshProUGUI livesText;

        public override void Setup(UI_ManagerBase _uiManager)
        {
            base.Setup(_uiManager);

            PlayerLivesController.OnLivesChange += HandleLivesChange;
        }

        public override void Enable()
        {
            base.Enable();

            if ((GameManager.Exist() && GameManager.instance.GetLevelsManager().GetMode()) || (LevelManager.Exist() && LevelManager.instance.GetSpeedrunManager().GetIsActive()))
                livesPanel.SetActive(false);
            else
                livesPanel.SetActive(true);
        }

        private void HandleLivesChange(int _lives)
        {
            livesText.text = "x" + _lives.ToString();
        }

        public void ToggleLivesText(bool _toggle)
        {
            livesPanel.SetActive(_toggle);
        }

        public void SetLivesText(int _value)
        {
            livesText.text = "x" + _value.ToString();
        }

        private void OnDestroy()
        {
            PlayerLivesController.OnLivesChange -= HandleLivesChange;
        }
    }
}
