using Common.Game.Math;
using MonogameBase;

namespace MonoGameBase.Editor
{

    public interface IMapWriter
    {
        public void SetSpawnPoint(int x, int y);
        public void SetTileAt(int x, int y, TileData data);
        public void AddEntity<T>(int x, int y, EntityIds id, T data) where T : EntityDataDTO;
        public (EntityIds, bool) RemoveEntityAt(int x, int y);
        public EntityIds GetEntityAt(int x, int y);

        public (uint visual, TileType type) GetTile(int x, int y);
    }
}
