using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Rarities;
using MogMod.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Flamewall : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const int DamageMult = 4;
        public const float CritMult = 2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageMult, CritMult.ToPercent());
        public override int ProjectileType => ModContent.ProjectileType<FlamewallHoldout>();
        public static Color WeakColor => new(255, 191, 41);
        public static Color StrongColor => new(255, 64, 0);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 134;

            Item.damage = 1150;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 40;
            Item.knockBack = 15f;
            Item.channel = true;
            Item.autoReuse = true;

            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = (46 * player.MogMod().flamewallPower);
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) return false;
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            List<Color> colorList =
            [
                new(214, 92, 92),
                new(209, 146, 59),
                new(217, 195, 74),
            ];
            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip5");
            if (line != null)
                line.OverrideColor = Color.Lerp(tooltipColor, Color.White, 0.5f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Daedalus>().
                AddIngredient(ItemID.TheAxe).
                AddIngredient<Flamebrand>().
                AddIngredient<VoniumBar>(5).
                AddIngredient<SoulOfMogMod>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}