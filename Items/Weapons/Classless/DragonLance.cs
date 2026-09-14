using MogMod.Common.Classes;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Classless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Classless
{
    public class DragonLance : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Classless";
        public override int ProjectileType => ModContent.ProjectileType<DragonLanceHoldout>();
        public const float DamageMult = 2f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 60;

            Item.damage = 114;
            Item.crit = 7;
            Item.DamageType = MeleeRangedDamageClass.Instance;
            Item.useAnimation = Item.useTime = 60;
            Item.knockBack = 8f;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) return false;
            return base.CanUseItem(player);
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Spear).
                AddIngredient<FuciumBar>(7).
                AddIngredient(ItemID.Ruby, 5).
                AddIngredient(ItemID.AntlionMandible, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}