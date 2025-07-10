using Coimbra.Services;
using Core.Scene.UI;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.SceneManagement;

namespace Core.Scene
{
    public class SceneLoader : ISceneLoader
    {
        public void Load(GameScene scene)
        {
            SceneManager.LoadScene((int)scene);
        }

        public async UniTask AsyncLoadWithLoader(GameScene scene, LoaderSceneParameters parameters = default)
        {
            if (parameters == null)
            {
                parameters = new();
            }

            var currentScene = SceneManager.GetActiveScene();

            await SceneManager.LoadSceneAsync((int)GameScene.Loader).ToUniTask();
            var loadScene = SceneManager.GetSceneByBuildIndex((int)GameScene.Loader);
            SceneManager.SetActiveScene(loadScene);

            UILoadSceneManager.Instance.Set(parameters);

            var asyncOp = SceneManager.LoadSceneAsync((int)scene);
            asyncOp.allowSceneActivation = false;

            await UniTask.WaitUntil(() => asyncOp.progress >= 0.9f);

            if (parameters.WaitForClick)
            {
                UILoadSceneManager.Instance.SetWaitForClick();
                UILoadSceneManager.Instance.SetText("");

                await UniTask.WaitUntil(() =>
                {
                    return UILoadSceneManager.Instance.CanAdvance;
                });
            }

            asyncOp.allowSceneActivation = true;
            await asyncOp.ToUniTask();
        }

        public void Dispose()
        {
        }
    }

    [RequiredService]
    public interface ISceneLoader : IService
    {
        public void Load(GameScene scene);
        public async UniTask AsyncLoadWithLoader(GameScene scene, LoaderSceneParameters parameters = default) { throw new NotImplementedException(); }
    }

    public enum GameScene
    {
        Initialization,
        MainMenu,
        Loader,
        Cave,
        BurrowHole
    }

    public record LoaderSceneParameters
    {
        public string InfoText { get; set; } = "Carregando";
        public bool WaitForClick { get; set; } = false;
        public bool DisableActiveScene { get; set; } = false;
    }
}