using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Magic;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MogMod.UI.TheGravityUI
{
    // TODO: make the unselected cards darker
    internal class TheGravitySpells : ModSystem
    {
        private Asset<Texture2D> card1;
        private Asset<Texture2D> emptyCard;
        internal const float TheGravityPosX = 340f;
        internal const float TheGravityPosY = 20;
        public static Asset<Texture2D>[] cardTexture;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                cardTexture = new Asset<Texture2D>[20];
                cardTexture[0] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityAutoCard");
                cardTexture[1] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityBookmarkCard");
                cardTexture[2] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityDefenseCard");
                cardTexture[3] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityFireCard");
                cardTexture[4] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityGravityCard");
                cardTexture[5] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityGravityStaffCard");
                cardTexture[6] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityIceCard");
                cardTexture[7] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityInstantHealthCard");
                cardTexture[8] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityInstantManaCard");
                cardTexture[9] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityMovementCard");
                cardTexture[10] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityOvertimeHealthCard");
                cardTexture[11] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityOvertimeManaCard");
                cardTexture[12] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityReplayCard");
                cardTexture[13] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityRepulsionStaffCard");
                cardTexture[14] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityShuffleCard");
                cardTexture[15] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravitySlowStaffCard");
                cardTexture[16] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravitySpeedStaffCard");
                cardTexture[17] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityTeleportCard");
                cardTexture[18] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityVoidCard");
                cardTexture[19] = ModContent.Request<Texture2D>("MogMod/UI/TheGravityUI/TheGravityWaterCard");
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
            int padding = 6;
            int iconSize = 20;

            card1 = cardTexture[TheGravity.CurrentCard];

            Texture2D tex1 = (mogPlayerUI.theGravityCurrent <= 3) ? emptyCard.Value : card1.Value;
            Texture2D tex2 = (mogPlayerUI.theGravityCurrent <= 2) ? emptyCard.Value : card1.Value;
            Texture2D tex3 = (mogPlayerUI.theGravityCurrent <= 1) ? emptyCard.Value : card1.Value;
            Texture2D tex4 = (mogPlayerUI.theGravityCurrent <= 0) ? emptyCard.Value : card1.Value;

            Vector2 pos1 = new Vector2(baseRight, baseTop);
            Vector2 pos2 = new Vector2(baseRight - (iconSize + padding), baseTop);
            Vector2 pos3 = new Vector2(baseRight - (iconSize + padding) * 2, baseTop);
            Vector2 pos4 = new Vector2(baseRight - (iconSize + padding) * 3, baseTop);

            if (Main.LocalPlayer.HeldItem.ModItem is TheGravity && MogClientConfig.Instance.TheGravitySpells)
            {
                spriteBatch.Draw(tex1, pos1, Color.White);
                spriteBatch.Draw(tex2, pos2, Color.White);
                spriteBatch.Draw(tex3, pos3, Color.White);
                spriteBatch.Draw(tex4, pos4, Color.White);
            }
        }
    }
}