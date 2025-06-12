using Coimbra.Services;
using Core.Units;
using System;

namespace Core.Train
{
    [DynamicService]
    public interface IColonyService : IService
    {
        public void AddGroupReward(float value);
        public void RegisterAnt(AntAgent agent);
        public void UnregisterAnt(AntAgent agent);
    }
}