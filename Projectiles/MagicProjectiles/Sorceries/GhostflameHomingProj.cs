using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Common.Classes;
using MogMod.Common.Graphics;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class GhostflameHomingProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public static Color Colour => new(227, 255, 253);
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 26;

            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = SorceryDamageClass.Instance;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.zenithWorld)
            {
                Projectile.timeLeft *= 5;
                Projectile.extraUpdates += 5;
            }
            Projectile.timeLeft = Projectile.MogMod().deathSpell ? (int)(Projectile.timeLeft * 1.2f) : Projectile.timeLeft;
        }
        public override void AI()
        {
            if (Main.zenithWorld)
                Projectile.extraUpdates += Projectile.MogMod().deathSpell ? 3 : 0;
            else
                Projectile.extraUpdates = Projectile.MogMod().deathSpell ? 3 : 0;
            float einstein = Projectile.MogMod().deathSpell ? 800f : 350f;
            if (Projectile.timeLeft < (570 * (Main.zenithWorld ? 5 : 1)))
                MogModUtils.HomeInOnNPC(Projectile, true, einstein, 2.5f, 18f, false);

            int ghostflameDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.Black, Color.White }), 1f);
            Main.dust[ghostflameDust].velocity *= 0.5f;
            Main.dust[ghostflameDust].scale *= 1.05f;
            Main.dust[ghostflameDust].fadeIn = 0.7f;
            Main.dust[ghostflameDust].noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GhostflameDebuff>(), 240);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GhostflameDebuff>(), 240);
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowTorch, 0f, 0f, 100, Utils.SelectRandom(Main.rand, new Color[] { Color.Black, Colour }), 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 1.2f;
                Main.dust[dust].velocity -= Projectile.oldVelocity * 0.3f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw trail
            TrailDrawer trailDrawer = default;
            trailDrawer.Draw(Projectile, "MogMod:FlameLashRGB", Color.White, Colour);

            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(tex, drawPosition, null, drawColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, direction);

            //MogModUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/CircleGradient").Value;
            Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour * 0.5f, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.3f, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
    }
}