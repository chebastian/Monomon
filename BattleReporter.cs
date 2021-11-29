using Microsoft.Xna.Framework.Graphics;
using Monomon.Battle;
using System;
using System.Collections.Generic;

namespace Monomon
{
    internal class BattleReporter : IBattleReporter
    {
        private readonly SpriteBatch batch;

        public List<string> Messages { get; set; }
        public BattleReporter(SpriteBatch batch)
        {
            if (batch is null)
            {
                throw new ArgumentNullException(nameof(batch));
            }

            this.batch = batch;
            Messages = new List<string>();
        }

        public void OnAttack(BattleMessage message)
        {
            Messages.Add($"{message.attacker} attacked {message.receiver} for {message.damage} points of damage!");
        }
    }
}