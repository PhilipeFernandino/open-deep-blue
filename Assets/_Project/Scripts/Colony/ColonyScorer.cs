using Coimbra;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Util;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

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
            var reward = _curriculumService.GetCurrentConfig().GetReward(e.AntEventType);

            if (reward.GroupReward != 0)
            {
                Debug.Log($"Assigning group reward: {reward.GroupReward}", this);
                _groupService.AddGroupReward(reward.GroupReward);
            }

            if (reward.AgentReward != 0)
            {
                Debug.Log($"Assigning agent reward: {reward.AgentReward} for ant: {e.Ant.gameObject.name}", this);
                e.Ant.Agent.AddReward(reward.AgentReward);
            }
        }

        private void ColonyEventHandler(ref EventContext context, in ColonyEvent e)
        {
            var reward = _curriculumService.GetCurrentConfig().GetReward(e.EventType);
            if (reward.GroupReward != 0)
            {
                Debug.Log($"Assigning group reward: {reward.GroupReward}", this);
                _groupService.AddGroupReward(reward.GroupReward);
            }

        }
    }
}