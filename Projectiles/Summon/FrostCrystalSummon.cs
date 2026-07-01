using System;
using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.Summon
{
    // code copied from calamity daedalus crystal
    public class FrostCrystalSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int dust = 3;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 46;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            bool isMinion = Projectile.type == ModContent.ProjectileType<FrostCrystalSummon>();
            Player player = Main.player[Projectile.owner];
            MogPlayer modPlayer = player.MogMod();
            if (!modPlayer.wearingFrostSummon)
            {
                Projectile.active = false;
                return;
            }
            if (isMinion)
            {
                if (player.dead)
                    modPlayer.fCrystal = false;
                if (modPlayer.fCrystal)
                    Projectile.timeLeft = 2;
            }
            dust--;
            if (dust >= 0)
            {
                int constant = 50;
                for (int i = 0; i < constant; i++)
                {
                    int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, DustID.Ice, 0f, 0f, 0, default, 1f);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].scale *= 1.15f;
                }
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.35f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.75f / 255f);
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                Projectile.position.Y += 120f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            float projScale = (float)Main.mouseTextColor / 200f - 0.35f;
            projScale *= 0.2f;
            Projectile.scale = projScale + 0.95f;
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0]--;
                    return;
                }
                bool isInRange = false;
                float projX = Projectile.Center.X;
                float projY = Projectile.Center.Y;
                float attackRange = 1000f;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float npcX = npc.position.X + (float)(npc.width / 2);
                        float npcY = npc.position.Y + (float)(npc.height / 2);
                        float npcDistance = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDistance < attackRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            projX = npcX;
                            projY = npcY;
                            isInRange = true;
                        }
                    }
                }
                if (!isInRange)
                {
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if (n.CanBeChasedBy(Projectile, false))
                        {
                            float otherNPCX = n.position.X + (float)(n.width / 2);
                            float otherNPCY = n.position.Y + (float)(n.height / 2);
                            float otherNPCDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - otherNPCX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - otherNPCY);
                            if (otherNPCDist < attackRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, n.position, n.width, n.height))
                            {
                                attackRange = otherNPCDist;
                                projX = otherNPCX;
                                projY = otherNPCY;
                                isInRange = true;
                            }
                        }
                    }
                }
                if (isInRange)
                {
                    float projXStore = projX;
                    float projYStore = projY;
                    projX -= Projectile.Center.X;
                    projY -= Projectile.Center.Y;
                    int projectileType = ProjectileID.Blizzard;
                    float randSpeed = Main.rand.Next(10, 15);
                    Vector2 firingDirection = Projectile.Center;
                    float projXDirection = projXStore - firingDirection.X;
                    float projYDirection = projYStore - firingDirection.Y;
                    float projSpeed = (float)Math.Sqrt((double)(projXDirection * projXDirection + projYDirection * projYDirection));
                    projSpeed = randSpeed / projSpeed;
                    projXDirection *= projSpeed;
                    projYDirection *= projSpeed;
                    int shard = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - 4f, Projectile.Center.Y, projXDirection, projYDirection, projectileType, Projectile.damage, 5f, Projectile.owner);
                    Main.projectile[shard].DamageType = Projectile.DamageType;
                    Projectile.ai[0] = 30f;
                }
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 200);
        }
        public override bool? CanDamage() => false;
    }
}