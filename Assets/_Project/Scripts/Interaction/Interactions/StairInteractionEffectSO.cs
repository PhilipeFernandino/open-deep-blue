using Core.Scene;
using Core.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Stair Interaction")]
    public class StairInteractionEffectSO : InteractionEffectSO
    {
        private ISceneLoader _sceneLoader;

        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            _sceneLoader = ServiceLocatorUtilities.GetServiceAssert<ISceneLoader>();

            if (interactor is Player.Player player)
            {
                Debug.Log($"Stair interaction: {player}", this);
                _sceneLoader.AsyncLoadWithLoader(GameScene.Cave).Forget();
            }
        }
    }
}