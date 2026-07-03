using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class ExplosiveGhostflame : SorcerySpell
    {
        public override int ManaCost => 36;
        public override int AttackSpeed => 58;
        public override SoundStyle UseSound => SoundID.Item73;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 88;
            Item.DamageType = DamageClass.Magic;
            Item.width = 50;
            Item.height = 52;
            Item.knockBack = 7f;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
            Item.shoot = ModContent.ProjectileType<ExplosiveGhostflameProj>();
            Item.shootSpeed = 7f;
        }
    }
}
