using Coimbra.Services.Events;
using Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Train
{
    public class ColonyScorer : MonoBehaviour
    {
        [SerializeField] private List<ColonyEventScore> _events = new();

        private Dictionary<ColonyEventType, float> _colonyEventScores = new();
        private IColonyService _groupService;

        private void Awake()
        {
            ColonyEvent.AddListener(ColonyEventHandler);

            foreach (var e in _events)
            {
                _colonyEventScores.Add(e.EventType, e.Score);
            }
        }

        private void Start()
        {
            _groupService = ServiceLocatorUtilities.GetServiceAssert<IColonyService>();
        }

        private void ColonyEventHandler(ref EventContext context, in ColonyEvent e)
        {
            _groupService.AddGroupReward(_colonyEventScores[e.EventType]);
        }
    }
}