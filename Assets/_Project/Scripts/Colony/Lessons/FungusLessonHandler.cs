namespace Core.Colony.Lessons
{
    using Core.Level;
    using Core.Map;
    using Core.Train;
    using Extensions;
    using System.Collections.Generic;
    using UnityEngine;

    public class FungusLessonHandler : LessonHandler
    {
        private List<Vector2Int> _fungusAntSpawnPoints;

        public FungusLessonHandler(IGridService gridService, LessonConfigSO config) : base(gridService, config)
        {
            _fungusAntSpawnPoints = new();
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
            mapMetadata.ListPositions(Tile.Fungus, _fungusAntSpawnPoints);

            mapMetadata.RemoveAll(Tile.CopperOre);
            mapMetadata.RemoveAll(Tile.Fungus);

            var randomFungusLocation = _fungusAntSpawnPoints.RandomElement();

            mapMetadata.SetTile(randomFungusLocation.x, randomFungusLocation.y, Tile.Fungus);

            new MapMetadataGeneratedEvent(mapMetadata).Invoke(this);
        }

        public override void OnExit() { }

        public override void HandleAntEvent(in AntEvent e)
        {
            if (e.AntEventType == AntEventType.Eat)
            {
                EndAgentEpisodeNextFrame(e.Ant.Agent);
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