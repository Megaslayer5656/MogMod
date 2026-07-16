using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Buffs.Debuffs;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class AghanimBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public Player Owner => Main.player[Projectile.owner];
        public Color Colour = new(153, 110, 255);
        public float velocityMult = 1f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.timeLeft = 1600;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
        }
        public override void OnSpawn(IEntitySource source) => velocityMult = Main.zenithWorld ? 0.1f: Main.rand.NextFloat(0.9f, 0.99f);
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Colour.ToVector3() * 0.5f);
            Dust dust = Dust.NewDustPerfect(Projectile.Center, 264, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.6f), 100);
            dust.noGravity = true;
            dust.scale = Main.rand.NextFloat(0.5f, 0.8f);
            dust.color = Main.rand.NextBool(3) ? Colour : Colour * 0.5f;

            if (Projectile.velocity.Length() < 4)
                Projectile.velocity += (Owner.MogMod().mouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 0.3f;
            else
                Projectile.velocity *= velocityMult;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // draw glow effect
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Texture2D bloomTex = ModContent.Request<Texture2D>("MogMod/Projectiles/BaseProjectiles/StarProj").Value;
            for (int i = 0; i < 2; i++)
            {
                Main.EntitySpriteDraw(bloomTex, drawPosition, null, Colour, Projectile.rotation, bloomTex.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None);

                if (Projectile.timeLeft <= 1550)
                {
                    // backtrail
                    Vector2 trailOffset = Projectile.oldVelocity * 10f;
                    for (float n = 0; n < 4; n++)
                    {
                        Color newColor = Colour * 0.25f;
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.2f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.2f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None);
                        Main.EntitySpriteDraw(bloomTex, drawPosition - (trailOffset * n * 0.4f), null, newColor with { A = 255 }, Projectile.oldRot[(int)(n * 0.4f)], bloomTex.Size() * 0.5f, Projectile.scale * 0.8f, SpriteEffects.None);
                    }
                }
            }
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AghanimHexDebuff>(), 600);
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AghanimHexDebuff>(), 600);
    }
}