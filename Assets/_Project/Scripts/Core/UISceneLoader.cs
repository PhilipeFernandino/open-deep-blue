using Coimbra;
using Coimbra.Services;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Core
{
    public class UISceneLoader : Actor, ISceneLoader
    {
        [SerializeField] private Button _continueButton;

        public void Load(Scene scene)
        {
            SceneManager.LoadScene((int)scene);
        }

        public async UniTask AsyncLoadWithLoader(Scene scene)
        {
            SceneManager.LoadScene((int)Scene.Load);
            await SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive).ToUniTask();
        }
    }

    [DynamicService]
    public interface ISceneLoader : IService
    {
        public void Load(Scene scene);
        public async UniTask AsyncLoadWithLoader(Scene scene) { throw new NotImplementedException(); }
    }

    public enum Scene
    {
        Menu,
        Load,
        Game,
    }
}