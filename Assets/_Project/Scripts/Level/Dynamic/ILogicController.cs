using UnityEngine;

namespace Core.Level.Dynamic
{
    public interface ILogicController { }

    public interface ILogicController<T, K> : ILogicController where T : struct, IDynamicTileData where K : struct
    {
        void OnUpdate(ref T data, K defData, Vector2Int position, IGridService grid, IChemicalGridService chemicals);
    }
}