using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class BoulderBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Boulder;
        }
        public override void AI()
        {
            // vanilla terraria boulder code

            // something in here makes the boulder turn on wall hit (which we dont want to happen)
            if (this.Projectile.ai[0] != 0f && Projectile.velocity.Y <= 0f && Projectile.velocity.X == 0f)
            {
                float num218 = 0.5f;
                int i2 = (int)((base.Projectile.position.X - 8f) / 16f);
                int num219 = (int)(base.Projectile.position.Y / 16f);
                bool flag9 = false;
                bool flag10 = false;
                if (WorldGen.SolidTile(i2, num219) || WorldGen.SolidTile(i2, num219 + 1))
                {
                    flag9 = true;
                }
                i2 = (int)((base.Projectile.position.X + (float)Projectile.width + 8f) / 16f);
                if (WorldGen.SolidTile(i2, num219) || WorldGen.SolidTile(i2, num219 + 1))
                {
                    flag10 = true;
                }
                if (flag9)
                {
                    Projectile.velocity.X = num218;
                }
                else if (flag10)
                {
                    Projectile.velocity.X = 0f - num218;
                }
                else
                {
                    i2 = (int)((base.Projectile.position.X - 8f - 16f) / 16f);
                    num219 = (int)(base.Projectile.position.Y / 16f);
                    flag9 = false;
                    flag10 = false;
                    if (WorldGen.SolidTile(i2, num219) || WorldGen.SolidTile(i2, num219 + 1))
                    {
                        flag9 = true;
                    }
                    i2 = (int)((base.Projectile.position.X + (float)Projectile.width + 8f + 16f) / 16f);
                    if (WorldGen.SolidTile(i2, num219) || WorldGen.SolidTile(i2, num219 + 1))
                    {
                        flag10 = true;
                    }
                    if (flag9)
                    {
                        Projectile.velocity.X = num218;
                    }
                    else if (flag10)
                    {
                        Projectile.velocity.X = 0f - num218;

                    }
                    else
                    {
                        i2 = (int)((base.Projectile.position.X - 8f - 32f) / 16f);
                        num219 = (int)(base.Projectile.position.Y / 16f);
                        flag9 = false;
                        flag10 = false;
                        if (WorldGen.SolidTile(i2, num219) || WorldGen.SolidTile(i2, num219 + 1))
                        {
                            flag9 = true;
                        }
                        i2 = (int)((base.Projectile.position.X + (float)Projectile.width + 8f + 32f) / 16f);
                        if (WorldGen.SolidTile(i2, num219) || WorldGen.SolidTile(i2, num219 + 1))
                        {
                            flag10 = true;
                        }
                        if (!flag9 && !flag10)
                        {
                            if ((int)(base.Projectile.Center.X / 16f) % 2 == 0)
                            {
                                flag9 = true;
                            }
                            else
                            {
                                flag10 = true;
                            }
                        }
                        if (flag9)
                        {
                            Projectile.velocity.X = num218;
                        }
                        else if (flag10)
                        {
                            Projectile.velocity.X = 0f - num218;
                        }
                    }
                }
            }

            // rotate boulder
            Projectile.rotation += Projectile.velocity.X * 0.06f;
            this.Projectile.ai[0] = 1f;
            
            // makes it heavy
            Projectile.velocity.Y = Projectile.velocity.Y + 0.20f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

            // hurt player after some time
            if (Projectile.timeLeft < 580)
            {
                Projectile.hostile = true;
            }

            // dust effect
            for (int k = 0; k < 1; k++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.3f);
                Main.dust[d].position = Projectile.Center;
                Main.dust[d].noLight = true;
            }
        }
        public override bool CanHitPvp(Player target) => false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Dazed, 180);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Dazed, 180);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // this makes the projectile bounce once

            if (Projectile.ai[0] == 1)
            {
                // how many times it can bounce

                // to make it bounce more than once use this code below

                //if (Projectile.ai[1] >= 1f)
                //{
                //    Projectile.Kill();
                //}
                //else
                //{
                    if (Projectile.ai[1] < 1f)
                    {
                        Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                        SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
                        if (Projectile.velocity.X != oldVelocity.X)
                        {
                            Projectile.velocity.X = -oldVelocity.X;
                        }

                        // bounce height
                        if (Projectile.velocity.Y != oldVelocity.Y)
                        {
                            Projectile.velocity.Y = -oldVelocity.Y * 0.4f;
                        }
                        Projectile.ai[1] += 1f;
                    }
                return false;
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }
}
