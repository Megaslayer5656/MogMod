using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class WingsOfLight : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(240, 9.5f, 2.7f);
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 38;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWingsOfLight = true;
            mogPlayer.wingsOfLightVisual = !hideVisual;
            player.moveSpeed += 0.1f;
            player.lavaImmune = true;
            player.noFallDmg = true;
            if (player.velocity.Y != 0f && !hideVisual)
            {
                int dustXOffset = 4;
                if (player.direction == 1)
                {
                    dustXOffset = -40;
                }
                int flightDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)dustXOffset, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, DustID.GoldCoin, 0f, 0f, 100, default, 2.4f);
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
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.GhostWings).
                AddIngredient(ItemID.GarlandHat).
                AddIngredient<FaeBar>(12).
                AddRecipeGroup("AnyScaleOrTissue", 9).
                AddIngredient<ManaCore>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}