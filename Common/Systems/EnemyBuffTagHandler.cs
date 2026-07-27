using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Utilities;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace MogMod.Common.Systems
{
    public sealed class EnemyBuffTagHandler : AbstractTagHandler<EnemyBuffTagHandler>
    {
        public sealed class Snippet(int buffId) : TextSnippet
        {
            private const float IconSize = 26f;

            public int BuffId => buffId;
            public bool DrawIcon => true;

            public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = new Vector2(), Color color = new Color(), float scale = 1)
            {
                size = new Vector2(GetStringLength(FontAssets.MouseText.Value), IconSize);

                if (!justCheckingString && (color.R != 0 || color.G != 0 || color.B != 0))
                {
                    // Avoid re-drawing the icon when drawing borders.
                    if (DrawIcon)
                    {
                        if (Main.netMode != NetmodeID.Server && !Main.dedServ)
                        {
                            var texture = TextureAssets.Buff[BuffId];
                            spriteBatch.Draw(texture.Value, new Rectangle((int)position.X, (int)position.Y - 2, (int)IconSize, (int)IconSize), null, Color.White);
                        }

                        position.X += IconSize;
                    }

                    var buffColor = ItemUtils.GetDebuffTooltipNameColor(buffId);
                    var name = $"{(DrawIcon ? " " : "")}{Lang.GetBuffName(buffId)}";
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, name, position, buffColor, 0f, Vector2.Zero, new Vector2(scale));
                }
                return true;
            }

            public override float GetStringLength(DynamicSpriteFont font)
            {
                float iconSize = !DrawIcon ? 0f : IconSize + font.MeasureString(" ").X;
                float size = iconSize + font.MeasureString(Lang.GetBuffName(buffId)).X;
                return size * Scale;
            }
        }

        protected override string[] TagNames { get; } = ["mogbuff"];

        public override TextSnippet Parse(string text, Color baseColor = new(), string options = null)
        {
            if (int.TryParse(text, out int buffId) && buffId >= 0 && buffId < BuffLoader.BuffCount)
            {
                return new Snippet(buffId);
            }

            if (BuffID.Search.TryGetId(text, out buffId))
            {
                return new Snippet(buffId);
            }

            return new TextSnippet(text);
        }
    }
}