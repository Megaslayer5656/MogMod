using Microsoft.Xna.Framework;
using MogMod.Buffs.Summons;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.Summon;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.Boots
{
    [AutoloadEquip(EquipType.Shoes)]
    public class UltraBootsOfTravel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Boots";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 34;
            Item.height = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.accRunSpeed = 11f;
            player.moveSpeed += .38f;
            player.rocketBoots = player.vanityRocketBoots = 3;

            // unique boot effects
            player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            player.waterWalk = true;
            player.iceSkate = true; // Grant the player improved speed on ice and not breaking thin ice when falling onto it
            player.desertBoots = true; // Grants the player increased movement speed while running on sand
            player.fireWalk = true; // Grants the player immunity from Meteorite and Hellstone tile damage
            player.noFallDmg = true; // Grants the player the Lucky Horseshoe effect of nullifying fall damage
            player.lavaRose = true; // Grants the Lava Rose effect
            player.lavaMax += 240; // Grants the player 4 additional seconds of lava immunity
            player.MogMod().wearingUltraTravelBoots = true;
            player.MogMod().ultraTravelBootsVisual = !hideVisual;

            var type = ModContent.ProjectileType<BootsTrailShaderProj>();
            if (!hideVisual)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    var source = player.GetSource_ItemUse(Item);
                    if (player.ownedProjectileCounts[type] < 1)
                    {
                        var p = Projectile.NewProjectileDirect(source, player.Center, Vector2.Zero, type, 1, 0f, Main.myPlayer);
                        p.active = true;
                        //p.ai[2] = 2f; // ultra boots of travel
                    }
                }

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
