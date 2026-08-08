using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class WingsOfAllegiance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";
        public const int WingTime = 360;
        public override void SetStaticDefaults() => ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(WingTime, 16f, 4f);
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingAllegianceWings = true;
            player.moveSpeed += 0.2f;
            player.noFallDmg = true;
            if (player.velocity.Y != 0f && !hideVisual)
            {
                int dustXOffset = 4;
                if (player.direction == 1)
                {
                    dustXOffset = -40;
                }
                int flightDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)dustXOffset, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, DustID.PlatinumCoin, 0f, 0f, 100, default, 2.4f);
                Main.dust[flightDust].noGravity = true;
                Main.dust[flightDust].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[flightDust].fadeIn = 2f;
                }
                Main.dust[flightDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 1f;
            ascentWhenRising = 0.2f;
            maxCanAscendMultiplier = 1.3f;
            maxAscentMultiplier = 3.5f;
            constantAscend = 0.2f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofFlight, 20).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}