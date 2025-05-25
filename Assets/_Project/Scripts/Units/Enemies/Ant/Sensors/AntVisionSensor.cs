using Core.Level;
using UnityEngine;

namespace Core.Units
{
    public class AntVisionSensor : AntSensor
    {
        public AntVisionSensor(Ant ant) : base(ant) { }

        public override void Sense()
        {
            Blackboard.Set(AntBlackboardKeys.CloseAnts, 5);
            Blackboard.Set(AntBlackboardKeys.PlayerPosition, Vector3.zero);
            PheromoneGrid.Drop(_ant.Position, AntPheromone.Scout, 5);
        }
    }
}