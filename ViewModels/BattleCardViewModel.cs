using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.ViewModels
{
    public class BattleCardViewModel
    {
        public BattleCardViewModel(string name, int maxHealth, int currentHealth, int level)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
            Level = level;
        }

        public string Name { get; }
        public int MaxHealth { get; }
        public int CurrentHealth { get; set; }
        public int Level { get; }
        public bool IsLow() => (float)CurrentHealth / (float)MaxHealth <= 0.25f;
    }
}
