using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.Rigs
{
    public class OspreyMK4A : ChestRig, IHoldShiftTooltipItem
    {
        public const float MovementSpeedBoost = 0.15f;
        public const float DamageReduction = 0.05f;
        public const int RangedCritBoost = 12;
        public const float AmmoReduction = 0.85f;
        //public const int MagSize = 65;
        //public const int MagReload = 80;
        public bool HidesNormalTooltip => true;
        public Color? TooltipExtensionColor => new(170, 170, 170);
        //public LocalizedText TooltipExtensionText => this.GetLocalization("HoldShiftTooltip").WithFormatArgs(MagSize, MagReload.FramesToSeconds());
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MovementSpeedBoost.ToPercent(), DamageReduction.ToPercent(), RangedCritBoost, AmmoReduction.ToReversedPercent());
        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.width = 30;
            Item.height = 20;

            Item.defense = 8;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.ammoCost *= AmmoReduction;
            //mogPlayer.maxShots = MagSize;
            //mogPlayer.reloadTime = MagReload;
            player.endurance += DamageReduction;
            player.GetCritChance<RangedDamageClass>() += RangedCritBoost;
            player.moveSpeed -= MovementSpeedBoost;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Main.zenithWorld)
                return Color.HotPink;
            return lightColor;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScavVest>().
                AddIngredient(ItemID.LunarTabletFragment, 8).
                AddIngredient<FrostEssence>(12).
                AddIngredient<SpookyEssence>(12).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}