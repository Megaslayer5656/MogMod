using MogMod.Common.Classes;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Items.Tools;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Classless
{
    public class HurricanePike : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Classless";
        ModKeybind keybindActive = null;
        public override int ProjectileType => ModContent.ProjectileType<HurricanePikeHoldout>();
        public const float DamageMult = 2.5f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 60;

            Item.damage = 246;
            Item.crit = 17;
            Item.DamageType = MeleeRangedDamageClass.Instance;
            Item.useAnimation = Item.useTime = 65;
            Item.knockBack = 10f;
            
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) return false;
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.FirstWeaponKeybind);
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DragonLance>().
                AddIngredient<ForceStaff>().
                AddIngredient(ItemID.ShroomiteBar, 15).
                AddIngredient(ItemID.SoulofFright, 7).
                AddIngredient(ItemID.Silk, 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}