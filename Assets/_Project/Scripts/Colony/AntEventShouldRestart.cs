using System;

namespace Core.Train
{
    [Serializable]
    public struct AntEventShouldRestart { public AntEventType EventType; public bool Restart; }
}