using Monomon.Mons;
using Monomon.State;
using Monomon.Views.Scenes;
using System;

namespace Monomon.Battle
{
    public record BattleMessage(string attacker, string receiver,string name, int damage);

    public abstract record ItemMessage(string user,string name)
    {
        public abstract void Use(Mobmon mon);
    }

    public record PotionMessage(string user, string name, int hpRestored) : ItemMessage(user, name)
    {
        public override void Use(Mobmon mon)
        {
            mon.Health = Math.Min(mon.Health + hpRestored,mon.MaxHealth);
        }
    }
}