namespace Core.Colony.Lessons
{
    using Core.Level;
    using Core.Map;
    using Core.Train;
    using Extensions;
    using System.Collections.Generic;
    using UnityEngine;

    public class Foraging101LessonHandler : LessonHandler
    {
        public Foraging101LessonHandler(IGridService gridService, LessonConfigSO config) : base(gridService, config)
        {
        }

        public override void OnEnter()
        {
            var tilemapAsset = Config.Tilemap;

            var mapMetadata = new MapMetadata(
                tilemapAsset.Tiles,
                tilemapAsset.BiomeTiles,
                new List<PointOfInterest>(),
                tilemapAsset.Dimensions,
                tilemapAsset.Name
            );


            mapMetadata.ListPositions(Tile.CopperOre, AntSpawnPoints);
            mapMetadata.RemoveAll(Tile.CopperOre);

            Debug.Log($"SpawnPoints Count: {AntSpawnPoints.Count}");

            new MapMetadataGeneratedEvent(mapMetadata).Invoke(this);
        }

        public override void OnExit() { }

        public override void HandleAntEvent(in AntEvent e)
        {
            if (e.AntEventType == AntEventType.GatherLeaf)
            {
                e.Ant.Agent.EndEpisode();
            }
        }

        public override void HandleColonyEvent(in ColonyEvent e)
        {

        }

        public override Vector2 GetSpawnPoint()
        {
            return AntSpawnPoints.RandomElement();
        }
    }
}