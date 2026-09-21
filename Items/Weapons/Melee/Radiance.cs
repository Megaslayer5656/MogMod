using MogMod.Items.Global;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Radiance : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public const int DamageMult = 4;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageMult);
        public override int ProjectileType => ModContent.ProjectileType<RadianceHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 72;

            Item.damage = 128;
            Item.crit = 37;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 60;
            Item.knockBack = 9f;
            Item.channel = true;
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
    }
}