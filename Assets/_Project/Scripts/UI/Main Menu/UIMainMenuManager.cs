using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UIMainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;

        [SerializeField] private Button _deleteGameButton;
        private void Awake()
        {
            _newGameButton.onClick.AddListener(NewGameAction);
        }

        public void NewGameAction()
        {

        }
    }
}
