using Coimbra;
using Coimbra.Services.Events;
using Core.Colony;
using Core.Level.Dynamic;
using Core.Util;
using UnityEngine;

namespace Core.Train
{

    public class ColonyScorer : Actor
    {
        private ICurriculumService _curriculumService;
        private IColonyService _groupService;
        private IQueenService _queenService;

        private bool _initialized = false;

        public void QueenDeath()
        {
            var reward = _curriculumService.GetCurrentConfig().GetReward(ColonyEventType.ColonyDeath);
            Debug.Log($"Queen death - Assigning group reward: {reward.GroupReward}", this);
            _groupService.AddGroupReward(reward.GroupReward);
        }

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
            _curriculumService.EnvironmentResetted += CurriculumEnvResetted;
        }

        private void CurriculumEnvResetted()
        {
            if (_initialized)
            { return; }

            _queenService = ServiceLocatorUtilities.GetServiceAssert<IQueenService>();
            _initialized = true;
        }

        private void FixedUpdate()
        {
            if (!_initialized)
                return;

            var currentConfig = _curriculumService.GetCurrentConfig();
            float groupReward = currentConfig.GroupExistentialReward;

            var (qp, queenData, queenDef) = _queenService.GetAny();

            if (qp == Vector2Int.zero)
            {
                return;
            }

            float saciation = queenData.CurrentSaciation / queenDef.MaxSaciation;
            groupReward += saciation * currentConfig.QueenSaciationReward;
            _groupService.AddGroupReward(groupReward * Time.fixedDeltaTime);
        }

        private void AntEventHandler(ref EventContext context, in AntEvent e)
        {
            if (!_initialized)
                return;

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
            if (!_initialized)
                return;

            var reward = _curriculumService.GetCurrentConfig().GetReward(e.EventType);
            if (reward.GroupReward != 0)
            {
                Debug.Log($"Assigning group reward: {reward.GroupReward}", this);
                _groupService.AddGroupReward(reward.GroupReward);
            }

        }
    }
}