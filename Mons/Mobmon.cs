using Monomon.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Mons
{
    public class Mobmon
    {
        public Mobmon(string name,int health, Battle.MonStatus stats)
        {
            Name = name;
            Health = health;
            MaxHealth = health;
            Stats = stats;
            Xp = 0;
        }

        public string Name { get; }
        public float Health { get; set; }
        public int MaxHealth { get; }
        public MonStatus Stats { get; }
        public float Xp { get; set; }
    }
}
