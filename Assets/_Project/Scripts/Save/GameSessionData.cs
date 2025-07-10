using UnityEngine;

namespace Core.Save
{
    public class GameSessionData
    {
        public Vector3 PlayerReturnPosition;
        public bool HasOverworldData;

        public void ResetData()
        {
            PlayerReturnPosition = Vector3.zero;
            HasOverworldData = false;
        }
    }

    public static class GameState
    {
        public static GameSessionData SessionData { get; private set; } = new();

        public static void SetSessionData(GameSessionData data)
        {
            SessionData = data;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void ResetData()
        {
            SessionData.ResetData();
        }
    }
}
