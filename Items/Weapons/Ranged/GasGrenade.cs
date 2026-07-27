using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class GasGrenade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 22;
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 80;
            Item.knockBack = 0.2f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 8f;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 22);
            Item.shoot = ModContent.ProjectileType<GasGrenadeProj>();
        }
    }
}