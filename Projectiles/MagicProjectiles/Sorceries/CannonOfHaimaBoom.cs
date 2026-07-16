using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.MagicProjectiles.Sorceries
{
    public class CannonOfHaimaBoom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic.Sorceries";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        private const float radius = 180f;
        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = SorceryDamageClass.Instance;
        }
        public override void AI()
        {
            int numb = 3;
            if (Main.zenithWorld)
                DestroyTiles();
            if (Projectile.timeLeft >= 8)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 50;

                    int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.HallowSpray, 0, 0, 100, default, 2.5f);
                    Dust dust = Main.dust[dagonDust];
                    dust.noGravity = true;
                    dust.position.X = Projectile.Center.X;
                    dust.position.Y = Projectile.Center.Y;
                    dust.position.X += (float)Main.rand.Next(-Projectile.width / numb, Projectile.width / numb);
                    dust.position.Y += (float)Main.rand.Next(-Projectile.height / numb, Projectile.height / numb);
                }
                for (int i = 0; i < 13; i++)
                {
                    Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 50;

                    int dagonDust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Electric, 0, 0, 100, default, 2.3f);
                    Dust dust = Main.dust[dagonDust];
                    dust.noGravity = true;
                    dust.position.X = Projectile.Center.X;
                    dust.position.Y = Projectile.Center.Y;
                    dust.position.X += (float)Main.rand.Next(-Projectile.width / numb, Projectile.width / numb);
                    dust.position.Y += (float)Main.rand.Next(-Projectile.height / numb, Projectile.height / numb);
                }
            }
        }
        private void DestroyTiles()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 destroyVector = Projectile.Center + Projectile.velocity * radius;
                int rad = 7;
                int mineXLeft = (int)(destroyVector.X / 16f - rad);
                int mineXRight = (int)(destroyVector.X / 16f + rad);
                int mineXUp = (int)(destroyVector.Y / 16f - rad);
                int mineXDown = (int)(destroyVector.Y / 16f + rad);
                if (mineXLeft < 0)
                    mineXLeft = 0;
                if (mineXRight > Main.maxTilesX)
                    mineXRight = Main.maxTilesX;
                if (mineXUp < 0)
                    mineXUp = 0;
                if (mineXDown > Main.maxTilesY)
                    mineXDown = Main.maxTilesY;
                AchievementsHelper.CurrentlyMining = true;
                for (int i = mineXLeft; i <= mineXRight; i++)
                    for (int j = mineXUp; j <= mineXDown; j++)
                    {
                        float destroyTileX = Math.Abs(i - destroyVector.X / 16f);
                        float destroyTileY = Math.Abs(j - destroyVector.Y / 16f);
                        double destroyTileArea = Math.Sqrt(destroyTileX * destroyTileX + destroyTileY * destroyTileY);
                        if (destroyTileArea < rad)
                            if (Main.tile[i, j] != null && Main.tile[i, j].HasTile)
                            {
                                WorldGen.KillTile(i, j, false, false, false);
                                if (!Main.tile[i, j].HasTile && Main.netMode != NetmodeID.SinglePlayer)
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j, 0f, 0, 0, 0);
                            }
                    }
                AchievementsHelper.CurrentlyMining = false;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => MogModUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}