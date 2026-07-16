using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using MogMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class GiantsMaul : NeutralItem
    {
        public const float SizeMult = 0.3f;
        public static double DamageMult = 2D;
        public const int DamageCap = 100;
        public const float MeleeSpeedBoost = 0.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SizeMult.ToPercent(), MeleeSpeedBoost.ToPercent(), DamageMult, DamageCap);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // increase size of melee weapons
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingGiantsMaul = true;

            player.GetAttackSpeed<MeleeDamageClass>() -= (Main.zenithWorld ? MeleeSpeedBoost * -3f : MeleeSpeedBoost);
        }
        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !MogModWorld.HasFoundGiantsMaul)
            {
                MogModWorld.HasFoundGiantsMaul = true;
                MogModNetcode.SyncWorld();
            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            int index = tooltips.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
            if (index != -1)
            {
                if (Main.zenithWorld)
                {
                    index++;
                    TooltipLine gfb = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<GiantsMaul>("TooltipGFB").Format(
                    SizeMult.ToPercent(), // {0}
                    MeleeSpeedBoost.ToPercent(), // {1}
                    DamageMult, // {2}
                    DamageCap)); // {3}
                    tooltips.Insert(index, gfb);
                }
                else
                {
                    index++;
                    TooltipLine normal = new(Mod, IHoldShiftTooltipItem.ExtensionIndicatorTooltipID, MiscUtils.GetTextFromModItem<GiantsMaul>("TooltipNormal").Format(
                    SizeMult.ToPercent(), // {0}
                    MeleeSpeedBoost.ToPercent(), // {1}
                    DamageMult, // {2}
                    DamageCap)); // {3}
                    tooltips.Insert(index, normal);
                }
            }
        }
        /* Moved to be guaranteed in a custom structure chest
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SkullBasher>(1).
                AddIngredient(ItemID.HellstoneBar, 12).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
        */
    }
}