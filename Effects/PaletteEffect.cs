using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Monomon.Effects
{
    public class PaletteEffect
    {
        private Effect paletteEffect;
        private Texture2D _palette;
        public PaletteEffect(ContentManager content, Texture2D paletteTexture)
        {
            paletteEffect = content.Load<Effect>("Indexed");

            _palette = paletteTexture;
            paletteEffect.Parameters["time"].SetValue(0.0f);
            paletteEffect.Parameters["swap"].SetValue(1.0f);
            paletteEffect.Parameters["palette"].SetValue(_palette);
        }

        public void EffectBegin(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, paletteEffect);
        }
    }
}
