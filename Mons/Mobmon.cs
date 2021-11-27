using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.Mons
{
    public class Mobmon
    {
        public Mobmon(string name,int health)
        {
            Name = name;
            Health = health;
            MaxHealth = health;
        }

        public string Name { get; }
        public int Health { get; set; }
        public int MaxHealth { get; }
    }
}
