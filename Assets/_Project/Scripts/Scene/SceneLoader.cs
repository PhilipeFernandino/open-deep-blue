using Coimbra;
using Coimbra.Services;
using Core.Scene.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Security.Cryptography;
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
            var currentScene = SceneManager.GetActiveScene();

            await SceneManager.LoadSceneAsync((int)GameScene.Load).ToUniTask();
            var loadScene = SceneManager.GetSceneByBuildIndex((int)GameScene.Load);
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
        Menu,
        Load,
        Game,
    }

    public record LoaderSceneParameters
    {
        public string InfoText { get; set; } = "Carregando";
        public bool WaitForClick { get; set; } = false;
        public bool DisableActiveScene { get; set; } = false;
    }
}