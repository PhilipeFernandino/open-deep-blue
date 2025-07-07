using Core.Map;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.RL
{
    [Serializable]
    public class DistributionTile { public Tile Tile; [Range(0f, 1f)] public float Distribution; }

    [CreateAssetMenu(menuName = "Core/Train/Lesson Config SO")]
    public class LessonConfigSO : ScriptableObject
    {
        [SerializeField, FormerlySerializedAs("_events")] private List<ColonyEventScore> _colonyEvents = new();
        [SerializeField] private List<AntEventScore> _antEvents = new();

        [SerializeField] private List<AntEventType> _shouldRestart = new();
        [SerializeField] private Reward _existentialReward;
        [SerializeField] private float _queenSaciationReward;

        [SerializeField] private TilemapAsset _tilemap;
        [SerializeField, FormerlySerializedAs("_maxStepsPerRound")] private int _maxStepsPerGroupEpisde = 5000;
        [SerializeField] private int _stepsPerEnvironmentReset = 5000;

        [SerializeField] private List<DistributionTile> _distributionTiles = new();

        private readonly Dictionary<ColonyEventType, Reward> _colonyEventScores = new();
        private readonly Dictionary<AntEventType, Reward> _antEventScores = new();

        private readonly Dictionary<Tile, float> _distributionTilesDic = new();
        private readonly HashSet<AntEventType> _antEventsShouldRestart = new();

        public TilemapAsset Tilemap => _tilemap;
        public List<ColonyEventScore> ColonyEvents => _colonyEvents;
        public List<AntEventScore> AntEvents => _antEvents;
        public int MaxStepsPerGroupEpisde => _maxStepsPerGroupEpisde;
        public int MaxStepPerEnvironmentReset => _stepsPerEnvironmentReset;

        public float AgentExistentialReward => _existentialReward.AgentReward;
        public float GroupExistentialReward => _existentialReward.GroupReward;
        public float QueenSaciationReward => _queenSaciationReward;


        private void OnEnable()
        {
            _colonyEventScores.Clear();
            _antEventScores.Clear();
            _antEventsShouldRestart.Clear();
            _distributionTilesDic.Clear();

            foreach (var e in _colonyEvents)
            {
                _colonyEventScores.Add(e.EventType, e.Reward);
            }

            foreach (var e in _antEvents)
            {
                _antEventScores.Add(e.EventType, e.Reward);
            }

            foreach (var e in _shouldRestart)
            {
                _antEventsShouldRestart.Add(e);
            }

            foreach (var e in _distributionTiles)
            {
                _distributionTilesDic.Add(e.Tile, e.Distribution);
            }
        }

        public Reward GetReward(ColonyEventType colonyEventType)
        {
            _colonyEventScores.TryGetValue(colonyEventType, out var score);
            return score;
        }

        public Reward GetReward(AntEventType antEventType)
        {
            _antEventScores.TryGetValue(antEventType, out var score);
            return score;
        }

        public float GetDistribution(Tile tile)
        {
            _distributionTilesDic.TryGetValue(tile, out var score);
            return score;
        }

        public bool Should(AntEventType antEventType)
        {
            return _antEventsShouldRestart.Contains(antEventType);
        }
    }
}