using Core.Level;
using Core.Map;
using Core.RL;
using System.Collections.Generic;
using UnityEngine;

namespace Core.RL
{
    [CreateAssetMenu(menuName = "Core/Reinforcement Learning/Curriculum/Lesson/Colony")]
    public class ColonyLessonHandler : LessonHandler
    {
        [SerializeField] private Bounds _spawnArea;

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

            List<Vector2> rPositions = new List<Vector2>();
            for (int i = 0; i < 3; i++)
            {
                Vector2 newPos;
                do
                {
                    newPos = _spawnArea.RandomPoint2D();
                } while (rPositions.Contains(newPos));
                rPositions.Add(newPos);
            }

            mapMetadata.SetTile((int)rPositions[0].x, (int)rPositions[0].y, Tile.GreenGrass);
            mapMetadata.SetTile((int)rPositions[1].x, (int)rPositions[1].y, Tile.Fungus);
            mapMetadata.SetTile((int)rPositions[2].x, (int)rPositions[2].y, Tile.QueenAnt);

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
            return _spawnArea.RandomPoint2D();
        }
    }
}
