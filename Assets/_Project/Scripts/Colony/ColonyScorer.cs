using Coimbra;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Util;
using UnityEngine;

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

            OnStarting += ColonyScorerOnStarting;
        }

        private void ColonyScorerOnStarting(Actor sender)
        {
            Debug.Log($"ColonyScorerOnStarting", this);
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
            var score = _curriculumService.GetCurrentConfig()?.GetReward(e.EventType) ?? 0;
            _groupService.AddGroupReward(score);
        }
    }
}