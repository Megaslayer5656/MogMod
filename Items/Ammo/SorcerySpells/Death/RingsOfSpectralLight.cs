using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Death
{
    public class RingsOfSpectralLight : SorcerySpell
    {
        public override int ManaCost => 22;
        public override int AttackSpeed => 72;
        public override int NumberOfAttacks => 6;
        public override int AttackDelay => 24;
        public override SoundStyle UseSound => SoundID.Item45;
        public int SpellNumb = 6;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 56;
            Item.knockBack = 5f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<SpectralLightProj>();
            Item.shootSpeed = 0f;
            SorceryClass = SorceryID.Death;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float angleVariance = MathHelper.TwoPi / 6;
            Vector2 projVec = new Vector2(5f, 0f).RotatedBy(MathHelper.ToRadians(60 * SpellNumb));
            projVec = projVec.RotatedBy(angleVariance);
            Projectile.NewProjectile(source, player.Center, projVec, type, damage, knockback, Main.myPlayer, ai1: SpellNumb);
            SpellNumb--;
            if (SpellNumb < 1)
                SpellNumb = 6;
            return false;
        }
    }
}