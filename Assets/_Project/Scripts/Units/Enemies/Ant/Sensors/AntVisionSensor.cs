using Core.Level;
using UnityEngine;

namespace Core.Units
{
    public class AntVisionSensor : AntSensor
    {
        public AntVisionSensor(Ant ant) : base(ant) { }

        public override void Sense()
        {
            Blackboard.SetInternal(AntBlackboardKeys.CloseAnts, 5);
            Blackboard.SetInternal(AntBlackboardKeys.PlayerPosition, Vector3.zero);
            PheromoneGrid.Drop(_ant.Position, AntPheromone.Scout, 5);
        }
    }
}