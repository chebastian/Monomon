using Common.Game.Math;
using MonoGameBase.Entity;
using System.Collections.Generic;

namespace MonogameBase.Level
{
    public abstract class LevelDataBase<Identifier>
    {
        public TileMap Map { get; }
        public Vec2 SpawnPoint { get; set; }
        public IEnumerable<Entity> Entities { get => entities; }

        protected List<Entity> entities;

        public List<Particle> Particles { get; }

        public List<EntityDataDTO> EntityDtos
        {
            get; private set;
        }
        public List<(uint start, uint mid, uint end, TileType type)> SpecialTiles { get; protected set; }
        public List<((int start, int len), TileType type, EntityIds entityId)> TypeMap { get; protected set; }
        public LevelDataBase(TileMap map)
        {
            Map = map;
            entities = new List<Entity>();
            EntityDtos = new List<EntityDataDTO>();
            Particles = new List<Particle>();
            SpawnPoint = new Vec2(5 * Constants.TileW, 3 * Constants.TileH);
        }

        public abstract void AddEntity<EntityData>(int x, int y, Identifier id, EntityData data) where EntityData : EntityDataDTO;
        public abstract void ClearEntities();
        public abstract (bool found, Identifier id) IndexToEntity(uint index);
    }
}
