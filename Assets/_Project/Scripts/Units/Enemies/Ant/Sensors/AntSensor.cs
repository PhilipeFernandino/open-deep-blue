using Core.Level;

namespace Core.Units
{
    public abstract class AntSensor
    {
        protected Ant _ant;

        public AntBlackboard Blackboard => _ant.Blackboard;
        public IPheromoneService PheromoneGrid => _ant.PheromoneGrid;

        public AntSensor(Ant ant)
        {
            _ant = ant;
        }

        public abstract void Sense();
    }
}