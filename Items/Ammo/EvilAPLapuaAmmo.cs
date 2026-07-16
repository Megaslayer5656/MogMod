using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo
{
    public class EvilAPLapuaAmmo : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";
        public override string Texture => "MogMod/Items/Ammo/EnergyBullet";
        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.DamageType = DamageClass.Ranged;
            Item.width = Item.height = 8;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.knockBack = 5f;
            Item.value = Item.sellPrice(copper: 22);
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<EvilAPLapua>();
            Item.shootSpeed = 5f;
            Item.ammo = ItemID.MusketBall;
        }
        // change the inventory sprite when in get fixed boi worlds
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (!Main.zenithWorld)
                return true;
            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Items/Ammo/EnergyBullet").Value;
            Color overlay = Color.PaleVioletRed;
            spriteBatch.Draw(texture, position, null, overlay, 0f, origin, scale, 0, 0);
            return false;
        }
        // change the dropped item sprite when in get fixed boi worlds
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            if (!Main.zenithWorld)
                return true;
            Texture2D texture = ModContent.Request<Texture2D>("MogMod/Items/Ammo/EnergyBullet").Value;
            Color overlay = Color.PaleVioletRed;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, overlay, 0f, Vector2.Zero, 1f, 0, 0);
            return false;
        }
        // change the tooltip when in get fixed boi worlds
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("Storm Shot", this.GetLocalizedValue(Main.zenithWorld ? "NameGFB" : "NameNormal"));
            var line = tooltips.FirstOrDefault(x => x.Text.Contains("[GFB]") && x.Mod == "Terraria");
            if (line != null)
            {
                line.Text = Lang.SupportGlyphs(this.GetLocalizedValue(Main.zenithWorld ? "TooltipGFB" : "TooltipNormal"));
                if (Main.zenithWorld)
                    line.OverrideColor = Main.DiscoColor;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe(500).
                AddIngredient(ItemID.NanoBullet, 500).
                AddIngredient<LizhardBloodVial>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}