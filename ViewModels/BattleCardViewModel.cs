using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monomon.ViewModels
{
    public class BattleCardViewModel
    {
        private float _time;
        private float _lastHealth;
        private float currentHealth;
        private float _healt;

        public BattleCardViewModel(string name, int maxHealth, float currentHealth, int level)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = currentHealth;
            Level = level;
            _lastHealth = currentHealth;
            _healt = currentHealth;
            _xp = 1; //TODO this should be set based on the mon
            _xpToNextLevel = 100;
        }

        public void Swap(string name, int maxH, float currentH, int level)
        {
            Name = name;
            CurrentHealth = currentH;
            MaxHealth = maxH;
            Level = level;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public string Name { get; private set; }
        public int MaxHealth { get; private set; }
        public float CurrentHealth
        {
            get => currentHealth; set
            {
                if (currentHealth != value)
                {
                    _lastHealth = currentHealth;
                    _time = 0.0f;
                }

                currentHealth = value;
            }
        }

        public void SetHealth(float h)
        {
            _healt = Math.Max(0, h);
            CurrentHealth = _healt;
            NewUpdate();
        }

        public void SetXp(float xp)
        {
            _xp = xp;
        }

        public void Update(float time)
        {
            var old = false;
            if (old)
            {
                _time += time;
                _time = MathF.Min(1.0f, _time);
                float fp = (float)_lastHealth / (float)MaxHealth;
                float ch = (float)currentHealth / (float)MaxHealth;
                Percentage = Lerp(ch, fp, 1.0f - _time);
            }
            else
                NewUpdate();
        }

        private void NewUpdate()
        {
            Percentage = (float)_healt / (float)MaxHealth;
            XpPercentage = (float)_xp / (float)_xpToNextLevel;
        }

        float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public int Level { get; private set; }
        public float Percentage { get; set; }
        public float XpPercentage { get; set; }
        public int PortraitOffsetX { get; internal set; }
        public int PortraitOffsetY { get; internal set; }
        public int PoirtrateAnimDelta { get; internal set; }
        public Rectangle PortraitSrc { get; internal set; }
        public bool Dying { get; internal set; }

        private float _xp;
        private float _xpToNextLevel;

        public bool IsLow() => (float)CurrentHealth / (float)MaxHealth <= 0.25f;
    }
}
