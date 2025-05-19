
namespace Core.Units
{
    public class HungerScorer : IScorer
    {
        public float Score(AntBlackboard blackboard)
        {
            float nutrition = blackboard.GetInternal<float>(AntBlackboardKeys.Nutrition);
            float hungerTolerance = blackboard.GetInternal<float>(AntBlackboardKeys.HungerTolerance);

            float distanceToFood = 0;

            return (1 - nutrition) * (1 - hungerTolerance) + distanceToFood * 0.3f;
        }
    }
}