using System.Collections;
using UnityEngine;
using static Systems.GridSystem.TileData;

namespace Systems.GridSystem
{
    public class GridTileData : MonoBehaviour
    {
        //public TileData TileData { get; private set; }
        //public TileInfo Info { get; private set; }
        //public TileOccupation Occupation { get; private set; }

        //public bool TryToOccupy(TileOccupation tileOccupation, bool isRoot)
        //{
        //    if (Info.HasFlag(TileInfo.IsOccupied))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        if (tileOccupation == TileOccupation.Plant && TileData.Properties.HasFlag(TilePropeties.Plantable))
        //        {
        //            Occupation = TileOccupation.Plant;
        //        }
        //        else if (tileOccupation == TileOccupation.Construction && TileData.Properties.HasFlag(TilePropeties.Constructable))
        //        {
        //            Occupation = TileOccupation.Construction;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }

        //    Info |= TileInfo.IsOccupied;
        //    return true;
        //}


        //[System.Flags]
        //public enum TileInfo
        //{
        //    None = 0,
        //    IsOccupied = 1,
        //    IsDestructible = 2,
        //    IsOccupationRoot = 4,
        //}

        //public enum TileOccupation
        //{
        //    Plant,
        //    Construction,
        //}
    }
}