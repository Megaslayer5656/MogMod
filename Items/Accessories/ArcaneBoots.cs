using MogMod.Common.Player;
using MogMod.Common.Systems;
using MogMod.Items.Other;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ArcaneBoots : ModItem, ILocalizedModType
    {
        int teamBuff = ModContent.BuffType<Buffs.PotionBuffs.GlimmerCapeBuff>();
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.ArcaneBootsKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // give mana boots an on button press affect that restores 200 mana and if possible does so to everyone
            player.moveSpeed += .15f;
            player.accRunSpeed = 7f;
            player.statManaMax2 += 50;
            player.manaRegen += (int)Math.Round(player.manaRegen * .2f);
            // a check on whether the player is wearing boots
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingManaBoots = true;

            // provides a buff to players on your team
            //if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            //{
            //    int myPlayer = Main.myPlayer;
            //    if (Main.player[myPlayer].team == player.team && player.team != 0)
            //    {
            //        float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
            //        float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
            //        if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < 800f)
            //            Main.player[myPlayer].statManaMax2 += 100;
            //            Main.player[myPlayer].AddBuff(teamBuff, 20);
            //    }
            //}

            // Determines whether the boots count as rocket boots
            // 0 - These are not rocket boots
            // Anything else - These are rocket boots
            player.rocketBoots = 2;

            // Sets which dust and sound to use for the rocket flight
            // 1 - Rocket Boots
            // 2 - Fairy Boots, Spectre Boots, Lightning Boots
            // 3 - Frostspark Boots
            // 4 - Terrraspark Boots
            // 5 - Hellfire Treads
            player.vanityRocketBoots = 2;

            // unique boot effects
            //player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            //player.waterWalk = true; // Allows walking on water, honey, and shimmer without falling into it
            //player.iceSkate = true; // Grant the player improved speed on ice and not breaking thin ice when falling onto it
            //player.desertBoots = true; // Grants the player increased movement speed while running on sand
            //player.fireWalk = true; // Grants the player immunity from Meteorite and Hellstone tile damage
            //player.noFallDmg = true; // Grants the player the Lucky Horseshoe effect of nullifying fall damage
            //player.lavaRose = true; // Grants the Lava Rose effect
            //player.lavaMax += LavaImmunityTime * 60; // Grants the player 2 additional seconds of lava immunity


            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects();

                // Hellfire Treads sprint dust. For more info on sprint dusts see Player.SpawnFastRunParticles() method in Player.cs
                player.coldDash = true;
                // Other boot run visual effects include: sailDash, coldDash, desertDash, fairyBoots

                // makes a fire trail behind you on the ground
                //if (!player.mount.Active || player.mount.Type != MountID.WallOfFleshGoat)
                //{
                //    player.DoBootsEffect(player.DoBootsEffect_PlaceFlamesOnTile);
                //}
            }
        }

        // this might do nothing idk
        private float Distance(Microsoft.Xna.Framework.Vector2 center1, Microsoft.Xna.Framework.Vector2 center2)
        {
            throw new NotImplementedException();
        }

        public override void AddRecipes()
        {
            // gives the item a recipe
            CreateRecipe().
                // add a vanilla item to the crafting recipe
                AddIngredient(ItemID.SpectreBoots, 1).
                AddIngredient(ItemID.ManaCrystal, 5).
                // add a modded item to the crafting recipe
                AddIngredient<CraftingRecipe>(1).
                // add where the item can be crafted
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
