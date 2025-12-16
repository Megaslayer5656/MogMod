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
    public class DrumOfEndurance : ModItem, ILocalizedModType
    {
        int teamBuff = ModContent.BuffType<Buffs.DrumOfEnduranceBuff>();
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.30f;
            player.GetAttackSpeed(DamageClass.Generic) += .15f;
            player.GetDamage(DamageClass.Melee) += -.30f;
            player.GetDamage(DamageClass.Summon) += .05f;
            player.maxMinions += 1;
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
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BeltOfStrength>(1).
                AddIngredient(ItemID.Robe, 1).
                AddIngredient(ItemID.AnkletoftheWind, 1).
                AddIngredient(ItemID.JungleSpores, 7).
                AddIngredient(ItemID.RichMahogany, 15).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}