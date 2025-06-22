namespace Core.Colony.Lessons
{
    using Core.Level;
    using Core.Map;
    using Core.Train;
    using Extensions;
    using System.Collections.Generic;
    using UnityEngine;

    public class QueenLessonHandler : LessonHandler
    {
        private List<Vector2Int> _queenAntSpawnPoints;
        private List<Vector2Int> _fungusAntSpawnPoints;


        public QueenLessonHandler(IGridService gridService, LessonConfigSO config) : base(gridService, config)
        {
            _queenAntSpawnPoints = new();
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
            mapMetadata.ListPositions(Tile.QueenAnt, _queenAntSpawnPoints);
            mapMetadata.ListPositions(Tile.Fungus, _fungusAntSpawnPoints);

            mapMetadata.RemoveAll(Tile.QueenAnt);
            mapMetadata.RemoveAll(Tile.Fungus);
            mapMetadata.RemoveAll(Tile.CopperOre);

            var randomFungusLocation = _fungusAntSpawnPoints.RandomElement();
            var randomQueenLocation = _queenAntSpawnPoints.RandomElement();

            mapMetadata.SetTile(randomFungusLocation.x, randomFungusLocation.y, Tile.Fungus);
            mapMetadata.SetTile(randomQueenLocation.x, randomQueenLocation.y, Tile.QueenAnt);

            new MapMetadataGeneratedEvent(mapMetadata).Invoke(this);
        }

        public override void OnExit() { }

        public override void HandleAntEvent(in AntEvent e)
        {

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