using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Magic;
using MogMod.Projectiles.MagicProjectiles.TheGravitySpells;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MogMod.UI.TheGravityUI
{
    internal class TheGravitySpells : ModSystem
    {
        private Asset<Texture2D> card1 = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityEmptyCard");
        private Asset<Texture2D> card2 = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityEmptyCard");
        private Asset<Texture2D> card3 = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityEmptyCard");
        private Asset<Texture2D> card4 = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityEmptyCard");
        private Asset<Texture2D> replay = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityReplay");
        private Asset<Texture2D> invis = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/InvisibleProj");
        private Asset<Texture2D> emptyCard;
        internal const float TheGravityPosX = 340f;
        internal const float TheGravityPosY = 20;
        public static Asset<Texture2D>[] cardTexture;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                cardTexture = new Asset<Texture2D>[20];
                // attack cards
                cardTexture[0] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityWaterCard");
                cardTexture[1] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityFireCard");
                cardTexture[2] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityIceCard");
                cardTexture[3] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityGravityCard");
                cardTexture[4] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityVoidCard");
                // player cards
                cardTexture[5] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityInstantHealthCard");
                cardTexture[6] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityInstantManaCard");
                cardTexture[7] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityOvertimeHealthCard");
                cardTexture[8] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityOvertimeManaCard");
                cardTexture[9] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityMovementCard");
                cardTexture[10] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityDefenseCard");
                // slot cards
                cardTexture[11] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityBookmarkCard");
                cardTexture[12] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityReplayCard");
                cardTexture[13] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityShuffleCard");
                // staff cards
                cardTexture[14] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravitySlowStaffCard");
                cardTexture[15] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravitySpeedStaffCard");
                cardTexture[16] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityGravityStaffCard");
                cardTexture[17] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityRepulsionStaffCard");
                // chaos cards
                cardTexture[18] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityAutoCard");
                cardTexture[19] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityTeleportCard");
                emptyCard = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityEmptyCard");

            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryLayer != -1)
            {
                // Draw before inventory so items/mouse hover over us
                layers.Insert(inventoryLayer, new LegacyGameInterfaceLayer(
                    "MogMod: TheGravitySpells",
                    delegate
                    {
                        DrawCustomIcons(Main.spriteBatch);
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
        private void DrawCustomIcons(SpriteBatch spriteBatch)
        {
            if (Main.player[Main.myPlayer].dead) return;

            var mogPlayerUI = Main.LocalPlayer.GetModPlayer<MogPlayerUI>();

            Vector2 posSlop = new Vector2(MogClientConfig.Instance.TheGravityPosX, MogClientConfig.Instance.TheGravityPosY);
            if (posSlop.X < 0f || posSlop.X > 1000f)
                posSlop.X = TheGravityPosX;
            if (posSlop.Y < -100f || posSlop.Y > 500)
                posSlop.Y = TheGravityPosY;

            //Vector2 screenPos = posSlop;
            //screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            //screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            float baseRight = Main.screenWidth - posSlop.X;
            float baseTop = posSlop.Y;
            float replayTop = posSlop.Y + 25f;
            int padding = 6;
            int iconSize = 20;

            card1 = cardTexture[TheGravity.Card1];
            card2 = cardTexture[TheGravity.Card2];
            card3 = cardTexture[TheGravity.Card3];
            card4 = cardTexture[TheGravity.Card4];

            Texture2D tex1 = (mogPlayerUI.theGravityCurrent1 < 1) ? emptyCard.Value : card1.Value;
            Texture2D tex2 = (mogPlayerUI.theGravityCurrent2 < 1) ? emptyCard.Value : card2.Value;
            Texture2D tex3 = (mogPlayerUI.theGravityCurrent3 < 1) ? emptyCard.Value : card3.Value;
            Texture2D tex4 = (mogPlayerUI.theGravityCurrent4 < 1) ? emptyCard.Value : card4.Value;

            Texture2D replayTex1 = (mogPlayerUI.theGravityReplay < 1 || mogPlayerUI.theGravityCurrent1 < 1) ? invis.Value : replay.Value;
            Texture2D replayTex2 = (mogPlayerUI.theGravityReplay < 1 || mogPlayerUI.theGravityCurrent2 < 1) ? invis.Value : replay.Value;
            Texture2D replayTex3 = (mogPlayerUI.theGravityReplay < 1 || mogPlayerUI.theGravityCurrent3 < 1) ? invis.Value : replay.Value;
            Texture2D replayTex4 = (mogPlayerUI.theGravityReplay < 1 || mogPlayerUI.theGravityCurrent4 < 1) ? invis.Value : replay.Value;

            Vector2 pos1 = new Vector2(baseRight, baseTop);
            Vector2 pos2 = new Vector2(baseRight - (iconSize + padding), baseTop);
            Vector2 pos3 = new Vector2(baseRight - (iconSize + padding) * 2, baseTop);
            Vector2 pos4 = new Vector2(baseRight - (iconSize + padding) * 3, baseTop);

            Vector2 replayPos1 = new Vector2(baseRight, replayTop);
            Vector2 replayPos2 = new Vector2(baseRight - (iconSize + padding), replayTop);
            Vector2 replayPos3 = new Vector2(baseRight - (iconSize + padding) * 2, replayTop);
            Vector2 replayPos4 = new Vector2(baseRight - (iconSize + padding) * 3, replayTop);

            if (Main.LocalPlayer.HeldItem.ModItem is TheGravity && MogClientConfig.Instance.TheGravitySpells)
            {
                spriteBatch.Draw(tex1, pos4, TheGravity.SwitchCard != 0 ? new Color(80, 80, 80) : Color.White);
                spriteBatch.Draw(tex2, pos3, TheGravity.SwitchCard != 1 ? new Color(80, 80, 80) : Color.White);
                spriteBatch.Draw(tex3, pos2, TheGravity.SwitchCard != 2 ? new Color(80, 80, 80) : Color.White);
                spriteBatch.Draw(tex4, pos1, TheGravity.SwitchCard != 3 ? new Color(80, 80, 80) : Color.White);

                spriteBatch.Draw(replayTex1, replayPos4, TheGravity.SwitchCard != 0 ? new Color(80, 80, 80) : Color.White);
                spriteBatch.Draw(replayTex2, replayPos3, TheGravity.SwitchCard != 1 ? new Color(80, 80, 80) : Color.White);
                spriteBatch.Draw(replayTex3, replayPos2, TheGravity.SwitchCard != 2 ? new Color(80, 80, 80) : Color.White);
                spriteBatch.Draw(replayTex4, replayPos1, TheGravity.SwitchCard != 3 ? new Color(80, 80, 80) : Color.White);
            }
        }
    }
}