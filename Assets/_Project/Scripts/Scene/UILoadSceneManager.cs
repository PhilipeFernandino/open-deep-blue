using Core.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scene.UI
{
    public class UILoadSceneManager : Singleton<UILoadSceneManager>
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private TextMeshProUGUI _infoTMP;
        [SerializeField] private TextMeshProUGUI _clickToContinueTMP;

        public Button ContinueButton => _continueButton;
        public TextMeshProUGUI InfoTMP => _infoTMP;
        public TextMeshProUGUI ClickToContinueTMP => _clickToContinueTMP;
        public bool CanAdvance { get; private set; } = false;

        private LoaderSceneParameters _parameters;

        public void Set(LoaderSceneParameters parameters)
        {
            _infoTMP.text = parameters.InfoText;
            _continueButton.gameObject.SetActive(false);
            _clickToContinueTMP.gameObject.SetActive(false);
            CanAdvance = false;
        }

        public void SetText(string text)
        {
            _infoTMP.text = text;
        }

        public void SetWaitForClick()
        {
            _continueButton.gameObject.SetActive(true);
            _clickToContinueTMP.gameObject.SetActive(true);
        }

        private void Start()
        {
            _continueButton.onClick.AddListener(() => CanAdvance = true);
        }
    }
}