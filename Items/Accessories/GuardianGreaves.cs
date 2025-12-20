using MogMod.Buffs;
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
    public class GuardianGreaves : ModItem, ILocalizedModType
    {
        int teamBuff = ModContent.BuffType<Buffs.GuardianGreavesAura>();
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.GuardianGreavesKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // give mana boots an on button press affect that restores 200 mana and if possible does so to everyone
            player.moveSpeed += .25f;
            player.accRunSpeed = 8.5f;
            player.statLifeMax2 += 20;
            player.statManaMax2 += 100;
            player.tileSpeed += .40f;
            player.lifeRegen += 6;
            player.statDefense += 5;
            player.GetDamage(DamageClass.Melee) += -.30f;
            player.GetDamage(DamageClass.Generic) += .10f;
            Player.tileRangeX = Player.tileRangeY += 3;
            // a check on whether the player is wearing boots
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGigaManaBoots = true;

            //provides a buff to players on your team
            if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                    float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                    if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < 800f)
                    {
                        Main.player[myPlayer].AddBuff(teamBuff, 20);
                    }
                }
            }
            player.rocketBoots = player.vanityRocketBoots = 3;
            player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            player.iceSkate = true; // Grant the player improved speed on ice and not breaking thin ice when falling onto it
            player.fireWalk = true; // Grants the player immunity from Meteorite and Hellstone tile damage
            player.lavaRose = true; // Grants the Lava Rose effect
            player.lavaMax += 240; // Grants the player 2 additional seconds of lava immunity

            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects();
                player.coldDash = true;
            }
        }

        // this might do nothing idk
        private float Distance(Microsoft.Xna.Framework.Vector2 center1, Microsoft.Xna.Framework.Vector2 center2)
        {
            throw new NotImplementedException();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArcaneBoots>(1).
                AddIngredient<Mekansm>(1).
                AddIngredient(ItemID.TerrasparkBoots, 1).
                AddRecipeGroup("CobaltBar", 5).
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
