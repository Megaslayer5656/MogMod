using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class VladmirsOffering : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        int teamBuff = ModContent.BuffType<Buffs.AccessoryAuras.VladmirsBuff>();
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.LightRed;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingVladimirs = true;
            player.lifeSteal *= 1.2f;
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
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ManaRegenerationBand, 1).
                AddIngredient<BladesOfAttack>(1).
                AddIngredient(ItemID.Silk, 12).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Flesh"}", 7).
                AddIngredient<ManaEssence>(3).
                AddIngredient(ItemID.Skull, 1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
