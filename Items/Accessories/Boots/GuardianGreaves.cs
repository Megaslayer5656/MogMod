using MogMod.Buffs.AccessoryAuras;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.Boots
{
    public class GuardianGreaves : ModItem, ILocalizedModType
    {
        // auras
        public const int LifeBoost = 20;
        public const int AuraManaBoost = 50;
        public const int LifeRegenBoost = 8;
        public const int DefenseBoost = 8;
        public const float MagicDamageBoost = 0.08f;
        // accessory buffs
        public const int ManaBoost = 50;
        // use effect
        public const int LifeHeal = 140;
        public const int ManaHeal = 300;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, LifeBoost, AuraManaBoost, LifeRegenBoost.ToRegenPerSecond(), DefenseBoost, MagicDamageBoost.ToPercent(), LifeHeal, ManaHeal);
        int teamBuff = ModContent.BuffType<GuardianGreavesAura>();
        public new string LocalizationCategory => "Items.Accessories.Boots";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.BootsKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // give mana boots an on button press affect that restores 200 mana and if possible does so to everyone
            player.accRunSpeed = 8.5f;
            player.moveSpeed += .25f;
            player.statManaMax2 += ManaBoost;
            player.tileSpeed += .40f;
            player.aggro -= 1000;
            player.manaRegen += (int)Math.Round(player.manaRegen * .5f);
            player.manaRegenDelay -= 4f;
            Player.tileRangeX = Player.tileRangeY += 3;
            // a check on whether the player is wearing boots
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGigaManaBoots = true;

            //provides a buff to players on your team
            if (player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                player.AddBuff(teamBuff, 20);
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                    float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                    if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                    {
                        Main.player[myPlayer].AddBuff(teamBuff, 20);
                    }
                }
            }
            player.rocketBoots = player.vanityRocketBoots = 3;
            player.waterWalk2 = true; // Allows walking on all liquids without falling into it
            player.waterWalk = true;
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
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArcaneBoots>().
                AddIngredient<Mekansm>().
                AddIngredient(ItemID.TerrasparkBoots).
                AddIngredient(ItemID.SoulofMight, 7).
                AddRecipeGroup("AnyCobaltBar", 5).
                AddIngredient<FrigidCrystal>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
