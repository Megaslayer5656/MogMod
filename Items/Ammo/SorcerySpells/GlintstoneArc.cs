using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class GlintstoneArc : SorcerySpell
    {
        public override int ManaCost => 7;
        public override int AttackSpeed => 36;
        public override SoundStyle UseSound => SoundID.Item8;
        public override void SetStaticDefaults() => ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.WizardHat;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 25;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 8f;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.shoot = ModContent.ProjectileType<GlintstoneArcProj>();
            Item.shootSpeed = 7f;
        }
    }
}
