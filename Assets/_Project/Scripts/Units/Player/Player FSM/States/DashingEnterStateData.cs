using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public class DashingEnterStateData : IEnterStateData
    {
        public readonly Vector3 Target;

        public DashingEnterStateData(Vector3 target)
        {
            Target = target;
        }
    }
}