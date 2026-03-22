using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Common.Config;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MogMod.UI.GunlanceUI
{
	// This custom UI will show whenever the player is holding the ExampleCustomResourceWeapon item and will display the player's custom resource amounts that are tracked in GunlanceAmmoPlayer
	// TODO: let the config determine where the sprites are placed
    internal class GunlanceAmmo : ModSystem
    {
        private Asset<Texture2D> loadedGun;
        private Asset<Texture2D> unloadedGun;
        internal const float GunlanceAmmoPosX = 340f;
        internal const float GunlanceAmmoPosY = 20;
        public override void Load()
        {
            if (!Main.dedServ)
            {
                loadedGun = ModContent.Request<Texture2D>("MogMod/UI/GunlanceUI/GunlanceLoadedAmmo");
                unloadedGun = ModContent.Request<Texture2D>("MogMod/UI/GunlanceUI/GunlanceUnLoadedAmmo");
            }
		}
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryLayer != -1)
            {
                // Draw before inventory so items/mouse hover over us
                layers.Insert(inventoryLayer, new LegacyGameInterfaceLayer(
                    "MogMod: GunlanceAmmo",
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

            Vector2 posSlop = new Vector2(MogClientConfig.Instance.GunlanceAmmoPosX, MogClientConfig.Instance.GunlanceAmmoPosY);
            if (posSlop.X < 0f || posSlop.X > 1000f)
                posSlop.X = GunlanceAmmoPosX;
            if (posSlop.Y < -100f || posSlop.Y > 500)
                posSlop.Y = GunlanceAmmoPosY;

            //Vector2 screenPos = posSlop;
            //screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            //screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            float baseRight = Main.screenWidth - posSlop.X;
            float baseTop = posSlop.Y;
            int padding = 6;
            int iconSize = 20;

            Texture2D tex1 = (mogPlayerUI.gunlanceCurrent <= 2) ? unloadedGun.Value : loadedGun.Value;
            Texture2D tex2 = (mogPlayerUI.gunlanceCurrent <= 1) ? unloadedGun.Value : loadedGun.Value;
            Texture2D tex3 = (mogPlayerUI.gunlanceCurrent <= 0) ? unloadedGun.Value : loadedGun.Value;

            Vector2 pos1 = new Vector2(baseRight, baseTop);
            Vector2 pos2 = new Vector2(baseRight - (iconSize + padding), baseTop);
            Vector2 pos3 = new Vector2(baseRight - (iconSize + padding) * 2, baseTop);

            if (Main.LocalPlayer.HeldItem.ModItem is Gunlance && MogClientConfig.Instance.GunlanceAmmo)
            {
                spriteBatch.Draw(tex1, pos1, Color.White);
                spriteBatch.Draw(tex2, pos2, Color.White);
                spriteBatch.Draw(tex3, pos3, Color.White);
            }
        }
    }
}