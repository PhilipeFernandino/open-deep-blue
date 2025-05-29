using Core.Level;
using Core.Util;
using UnityEngine;

namespace Core.Interaction
{
    public class BurrowHoleInteractable : ScriptableObject, IInteractable
    {
        public void Interact(Player.Player player)
        {

        }
    }

    public class BurrowHoleInteractableController : Singleton<BurrowHoleInteractableController>, IInteractable
    {
        private IGridService _gridService;

        public void Interact(Player.Player player)
        {

        }

        private void Start()
        {
            _gridService = ServiceLocatorUtilities.GetServiceAssert<IGridService>();
        }
    }
}