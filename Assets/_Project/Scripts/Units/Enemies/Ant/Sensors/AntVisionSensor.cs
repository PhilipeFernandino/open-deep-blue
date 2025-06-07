using Core.Level;
using UnityEngine;

namespace Core.Units
{
    public class AntVisionSensor : AntSensor
    {
        public AntVisionSensor(Ant ant) : base(ant) { }

        public override void Sense()
        {
            return;

            PheromoneGrid.Drop(_ant.Position, AntPheromone.Explore, 5);
        }
    }
}