using Core.Save;
using Core.Scene;
using Core.Util;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Interaction
{
    [CreateAssetMenu(menuName = "Core/Interactions/Burrow Hole")]
    public class BurrowHoleInteractionEffectSO : InteractionEffectSO
    {
        private ISceneLoader _sceneLoader;

        public override void Execute(MonoBehaviour interactor, Vector2 worldPosition)
        {
            _sceneLoader = ServiceLocatorUtilities.GetServiceAssert<ISceneLoader>();

            if (interactor is Player.Player player)
            {
                Debug.Log($"Burrow hole interaction: {player}", this);

                GameState.SessionData.PlayerReturnPosition = worldPosition;
                GameState.SessionData.HasOverworldData = true;

                _sceneLoader.AsyncLoadWithLoader(GameScene.BurrowHole).Forget();
            }
        }
    }
}