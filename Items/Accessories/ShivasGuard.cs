using MogMod.Common.Player;
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
    public class ShivasGuard : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.ShivasKeybind);
        ModKeybind keybindActive = null;
        int teamBuff = ModContent.BuffType<Buffs.AccessoryAuras.ShivasGuardBuff>();
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Cyan;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingShivasGuard = true;
            player.statDefense += 10;
            player.GetDamage(DamageClass.Ranged) -= .30f;
            player.GetDamage(DamageClass.Magic) += .10f;
            player.GetDamage(DamageClass.Generic) += .10f;
            player.GetAttackSpeed(DamageClass.Generic) += .10f;
            player.lifeRegen += 4;
            player.statManaMax2 += 50;
            player.statLifeMax2 += 50;
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
                AddIngredient<VeilOfDiscord>(1).
                AddRecipeGroup("AdamantiteBar", 25).
                AddIngredient(ItemID.SoulofMight, 15).
                AddIngredient(ItemID.FrostCore, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
