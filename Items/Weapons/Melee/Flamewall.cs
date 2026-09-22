using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace MogMod.Items.Weapons.Melee
{
    public class Flamewall : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const int DamageMult = 4;
        //public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageMult);
        public override int ProjectileType => ModContent.ProjectileType<FlamewallHoldout>();
        public static Color WeakColor => new(255, 191, 41);
        public static Color StrongColor => new(255, 64, 0);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 134;

            Item.damage = 350;
            Item.crit = 46;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 100;
            Item.knockBack = 15f;
            Item.channel = true;
            Item.autoReuse = true;

            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = (crit * player.MogMod().flamewallPower) + 10;
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage = (damage * (player.MogMod().flamewallPower + 0.5f));
        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback) => knockback += player.MogMod().flamewallPower;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) return false;
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            List<Color> colorList = new List<Color>()
            {
                new Color(214, 92, 92),
                new Color(209, 146, 59),
                new Color(217, 195, 74),
            };
            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip4");
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