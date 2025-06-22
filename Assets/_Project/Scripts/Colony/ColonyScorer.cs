using Coimbra;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Util;

namespace Core.Train
{

    public class ColonyScorer : Actor
    {
        private ICurriculumService _curriculumService;
        private IColonyService _groupService;

        protected override void OnInitialize()
        {
            ColonyEvent.AddListener(ColonyEventHandler);
            AntEvent.AddListener(AntEventHandler);
        }

        protected override void OnSpawn()
        {
            _groupService = ServiceLocatorUtilities.GetServiceAssert<IColonyService>();
            _curriculumService = ServiceLocatorUtilities.GetServiceAssert<ICurriculumService>();
        }

        private void AntEventHandler(ref EventContext context, in AntEvent e)
        {
            var score = _curriculumService.GetCurrentConfig().GetReward(e.AntEventType);
            e.Ant.GiveReward(score);
        }

        private void ColonyEventHandler(ref EventContext context, in ColonyEvent e)
        {
            var score = _curriculumService.GetCurrentConfig().GetReward(e.EventType);
            _groupService.AddGroupReward(score);
        }
    }
}