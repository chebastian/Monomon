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
            _healt = maxHealth;
        }

        public string Name { get; }
        public int MaxHealth { get; }
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
            _healt = h;
            CurrentHealth = h;
            NewUpdate();
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
        }

        float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        public int Level { get; }
        public float Percentage { get; set; }

        public bool IsLow() => (float)CurrentHealth / (float)MaxHealth <= 0.25f;
    }
}
