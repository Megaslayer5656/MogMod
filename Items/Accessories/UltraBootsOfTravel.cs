using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class UltraBootsOfTravel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += .45f;
            player.accRunSpeed = 12f;
            player.rocketBoots = player.vanityRocketBoots = 3;

            // unique boot effects
            player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            player.iceSkate = true; // Grant the player improved speed on ice and not breaking thin ice when falling onto it
            player.desertBoots = true; // Grants the player increased movement speed while running on sand
            player.fireWalk = true; // Grants the player immunity from Meteorite and Hellstone tile damage
            player.noFallDmg = true; // Grants the player the Lucky Horseshoe effect of nullifying fall damage
            player.lavaRose = true; // Grants the Lava Rose effect
            player.lavaMax += 240; // Grants the player 2 additional seconds of lava immunity


            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects();

                // Hellfire Treads sprint dust. For more info on sprint dusts see Player.SpawnFastRunParticles() method in Player.cs
                player.fairyBoots = true;
                // Other boot run visual effects include: sailDash, coldDash, desertDash, fairyBoots

                // makes a fire trail behind you on the ground
                //if (!player.mount.Active || player.mount.Type != MountID.WallOfFleshGoat)
                //{
                //    player.DoBootsEffect(player.DoBootsEffect_PlaceFlamesOnTile);
                //}
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BootsOfTravel>(1).
                AddIngredient(ItemID.TerrasparkBoots, 1).
                AddIngredient(ItemID.SandBoots, 1).
                AddIngredient(ItemID.HellstoneBar, 15).
                AddIngredient(ItemID.SwiftnessPotion, 5).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
