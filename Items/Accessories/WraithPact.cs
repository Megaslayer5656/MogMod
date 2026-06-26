using MogMod.Common.MogModPlayer;
using MogMod.Items.Consumables;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    // TODO: give this an active that does something cool
    public class WraithPact : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        int wraithBuff = ModContent.BuffType<Buffs.AccessoryAuras.WraithPactAura>();
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingWraithPact = true;
            player.lifeSteal *= 1.5f;
            if (player.miscCounter % 10 == 0)
            {
                int myPlayer = Main.myPlayer;
                player.AddBuff(wraithBuff, 20);
                if (Main.player[myPlayer].team == player.team && player.team != 0)
                {
                    float teamPlayerXDist = player.position.X - Main.player[myPlayer].position.X;
                    float teamPlayerYDist = player.position.Y - Main.player[myPlayer].position.Y;
                    if ((float)Math.Sqrt(teamPlayerXDist * teamPlayerXDist + teamPlayerYDist * teamPlayerYDist) < mogPlayer.auraRange)
                    {
                        Main.player[myPlayer].AddBuff(wraithBuff, 20);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VladmirsOffering>().
                AddIngredient(ItemID.AvengerEmblem).
                AddIngredient<GriefBar>(12).
                AddIngredient<PointBooster>().
                AddIngredient<SoulOfMogMod>().
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
