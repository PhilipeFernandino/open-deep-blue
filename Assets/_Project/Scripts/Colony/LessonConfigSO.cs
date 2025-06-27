using Core.Map;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Train
{

    [CreateAssetMenu(menuName = "Core/Train/Lesson Config SO")]
    public class LessonConfigSO : ScriptableObject
    {
        [SerializeField] private List<ColonyEventScore> _events = new();
        [SerializeField] private List<AntEventScore> _antEvents = new();
        [SerializeField] private List<AntEventType> _shouldRestart = new();
        [SerializeField] private TilemapAsset _tilemap;
        [SerializeField] private int _maxStepsPerRound = 5000;

        private Dictionary<ColonyEventType, float> _colonyEventScores = new();
        private Dictionary<AntEventType, float> _antEventScores = new();
        private HashSet<AntEventType> _antEventsShouldRestart = new();

        public TilemapAsset Tilemap => _tilemap;
        public List<ColonyEventScore> ColonyEvents => _events;
        public List<AntEventScore> AntEvents => _antEvents;
        public int MaxStepsPerRound => _maxStepsPerRound;


        private void OnEnable()
        {
            _colonyEventScores.Clear();
            _antEventScores.Clear();

            foreach (var e in _events)
            {
                _colonyEventScores.Add(e.EventType, e.Score);
            }

            foreach (var e in _antEvents)
            {
                _antEventScores.Add(e.EventType, e.Score);
            }

            foreach (var e in _shouldRestart)
            {
                _antEventsShouldRestart.Add(e);
            }
        }

        public float GetReward(ColonyEventType colonyEventType)
        {
            _colonyEventScores.TryGetValue(colonyEventType, out var score);
            return score;
        }

        public float GetReward(AntEventType antEventType)
        {
            _antEventScores.TryGetValue(antEventType, out var score);
            return score;
        }

        public bool Should(AntEventType antEventType)
        {
            return _antEventsShouldRestart.Contains(antEventType);
        }
    }
}