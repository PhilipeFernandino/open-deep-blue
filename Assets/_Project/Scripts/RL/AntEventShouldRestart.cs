using System;

namespace Core.RL
{
    [Serializable]
    public struct AntEventShouldRestart { public AntEventType EventType; public bool Restart; }
}