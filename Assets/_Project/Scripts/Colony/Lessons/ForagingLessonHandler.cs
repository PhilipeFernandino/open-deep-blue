namespace Core.Colony.Lessons
{
    using Core.Level;
    using Core.Map;
    using Core.Train;
    using Cysharp.Threading.Tasks;
    using Extensions;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Core/Curriculum/Lesson/Foraging")]
    public class ForagingLessonHandler : LessonHandler
    {
        private List<Vector2Int> _allGrassLocations = new();

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

            SetupProceduralFood(mapMetadata);

            new MapMetadataGeneratedEvent(mapMetadata).Invoke(this);
        }

        private void SetupProceduralFood(MapMetadata map)
        {
            _allGrassLocations.Clear();
            map.ListPositions(Tile.GreenGrass, _allGrassLocations);
            map.RemoveAll(Tile.GreenGrass);

            int numberOfPatchesToSpawn = Mathf.RoundToInt((float)_allGrassLocations.Count * Config.GetDistribution(Tile.GreenGrass));
            var selectedSpawnPoints = new List<Vector2Int>();

            for (int i = 0; i < numberOfPatchesToSpawn; i++)
            {
                if (_allGrassLocations.Count == 0)
                    break;

                var chosenPoint = _allGrassLocations.RandomElement();
                selectedSpawnPoints.Add(chosenPoint);
                _allGrassLocations.Remove(chosenPoint);
            }

            foreach (var spawnPoint in selectedSpawnPoints)
            {
                SpawnFoodPatch(map, spawnPoint, 3);
            }
        }

        private void SpawnFoodPatch(MapMetadata map, Vector2Int center, int size)
        {
            int half = size / 2;
            for (int x = -half; x < half; x++)
            {
                for (int y = -half; y < half; y++)
                {
                    var tilePosition = new Vector2Int(center.x + x, center.y + y);
                    map.SetTile(tilePosition.x, tilePosition.y, Tile.GreenGrass);
                }
            }
        }

        public override void OnExit() { }

        public override void HandleAntEvent(in AntEvent e)
        {
            if (e.AntEventType == AntEventType.GatherLeaf)
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