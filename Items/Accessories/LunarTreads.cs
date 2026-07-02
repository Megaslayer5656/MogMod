using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class LunarTreads : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(200, 11f, 2.8f, true, 13f, 13f);
            
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.15f;
            player.lavaImmune = true;

            player.accRunSpeed = 15f;
            player.moveSpeed += .5f;
            player.rocketBoots = player.vanityRocketBoots = 4;

            // unique boot effects
            player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            player.waterWalk = true;
            player.iceSkate = true; // Grant the player improved speed on ice and not breaking thin ice when falling onto it
            player.desertBoots = true; // Grants the player increased movement speed while running on sand
            player.fireWalk = true; // Grants the player immunity from Meteorite and Hellstone tile damage
            player.noFallDmg = true; // Grants the player the Lucky Horseshoe effect of nullifying fall damage
            player.lavaRose = true; // Grants the Lava Rose effect

            if (player.controlJump && player.wingTime > 0f && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                player.CancelAllBootRunVisualEffects();
                player.fairyBoots = true;
                int dustXOffset = 4;
                if (player.direction == 1)
                    dustXOffset = -40;
                int flightDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)dustXOffset, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, DustID.Terragrim, 0f, 0f, 100, default, 2.4f);
                Main.dust[flightDust].noGravity = true;
                Main.dust[flightDust].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                    Main.dust[flightDust].fadeIn = 2f;
                Main.dust[flightDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            maxCanAscendMultiplier = 1.15f;
            maxAscentMultiplier = 3f;
            if (player.TryingToHoverDown && player.controlJump && player.wingTime > 0f && !player.merman)
            {
                player.wingTime += 0.5f;
                player.velocity.Y *= 0.8f;
                if (player.velocity.Y > -2f && player.velocity.Y < 1f)
                    player.velocity.Y = 1E-05f;

                ascentWhenFalling *= 0f;
                ascentWhenRising *= 0f;
                constantAscend *= 0f;
                return;
            }
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            constantAscend = 0.15f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UltraBootsOfTravel>(1).
                AddIngredient(ItemID.SoulofFlight, 20).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}