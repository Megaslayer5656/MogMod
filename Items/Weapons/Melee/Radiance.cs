using MogMod.Common.Classes;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    // 72x72
    // hold up in air to create aura, dealing burn damage over time and increasing charge
    // tap right click while channeling to swing, dealing more damage the more charge there is
    public class Radiance : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<RadianceHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 72;

            Item.damage = 114;
            Item.crit = 37;
            Item.DamageType = MeleeRangedDamageClass.Instance;
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
