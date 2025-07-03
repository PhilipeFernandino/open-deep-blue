using Core.Scene;
using Core.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class UIMainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;

        private ISceneLoader _sceneLoader;

        private void Awake()
        {
            _newGameButton.onClick.AddListener(NewGameAction);
        }

        private void Start()
        {
            _sceneLoader = ServiceLocatorUtilities.GetServiceAssert<ISceneLoader>();
        }

        public void NewGameAction()
        {
            _sceneLoader.AsyncLoadWithLoader(Scene.GameScene.Game,
                new() { InfoText = "Carregando mapa...", WaitForClick = true, DisableActiveScene = true }).Forget();
        }
    }
}
