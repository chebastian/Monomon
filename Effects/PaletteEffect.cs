using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Monomon.Effects
{
    public class PaletteEffect
    {
        private Effect paletteEffect;
        private Texture2D _palette;
        private float currentPalette;

        public PaletteEffect(ContentManager content, Texture2D paletteTexture)
        {
            paletteEffect = content.Load<Effect>("Indexed");

            _palette = paletteTexture;
            paletteEffect.Parameters["time"].SetValue(0.0f);
            paletteEffect.Parameters["swap"].SetValue(1.0f);
            paletteEffect.Parameters["palette"].SetValue(_palette);
        }

        public float CurrentPalette
        {
            get => currentPalette; set
            {
                currentPalette = value;
                paletteEffect.Parameters["time"].SetValue(value);
            }
        }

        public void EffectBegin(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, paletteEffect);
        }
    }
}
