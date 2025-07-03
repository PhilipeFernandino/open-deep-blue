using Core.FSM;
using UnityEngine;

namespace Core.Player
{
    public class DashingEnterStateData : IEnterStateData
    {
        public readonly Vector2 Direction;
        public readonly float DashSpeed;
        public readonly float DashDuration;

        public DashingEnterStateData(Vector2 direction, float dashSpeed, float dashDuration)
        {
            Direction = direction;
            DashSpeed = dashSpeed;
            DashDuration = dashDuration;
        }
    }
}