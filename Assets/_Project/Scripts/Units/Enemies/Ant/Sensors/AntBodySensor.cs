namespace Core.Units
{
    public class AntBodySensor : AntSensor
    {
        public AntBodySensor(Ant ant) : base(ant) { }

        public override void Sense()
        {
            float hunger = _ant.Blackboard.Get<float>(AntBlackboardKeys.Nutrition);
            _ant.Blackboard.Set<float>(AntBlackboardKeys.Nutrition, hunger - 0.2f);
        }
    }
}