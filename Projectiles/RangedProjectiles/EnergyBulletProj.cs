using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class EnergyBulletProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public bool initialized = false;
        List<NPC> npcList = new List<NPC>();
        public int closestIndex = -1;
        public NPC currentTarget;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 600;
            Projectile.light = .5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1f;

            AIType = ProjectileID.Bullet;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) //This stuff like basically works, I'll polish it later
        {
            Projectile.netUpdate = true;
            if (!initialized)
            {
                npcList.Clear(); //Just in case

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.active && !npc.townNPC && npc.whoAmI != target.whoAmI && !npc.dontTakeDamage)
                    {
                        if (Microsoft.Xna.Framework.Vector2.Distance(Projectile.Center, npc.Center) < 2000f)
                        {
                            npcList.Add(npc);
                        }
                    }
                }

                initialized = true;
            }

            if (npcList.Count == 0)
            {
                currentTarget = null;
                return;
            }

            float closestDist = float.MaxValue;

            for (int i = 0; i < npcList.Count; i++)
            {
                NPC npc = npcList[i];

                if (!npc.active)
                    continue;

                float dist = Vector2.DistanceSquared(Projectile.Center, npc.Center);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestIndex = i;
                }
            }

            if (closestIndex == -1)
                return;

            currentTarget = npcList[closestIndex];
            npcList.RemoveAt(closestIndex);

            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, .5f);
                Main.dust[d].position = Projectile.Center;
            }
        }

        public override void AI()
        {
            Projectile.netUpdate = true;
            Projectile.netImportant = true;
            if (currentTarget == null)
                return;


            if (!currentTarget.active || currentTarget.dontTakeDamage)
            {
                if (npcList.IndexOf(currentTarget) == npcList.Count - 1)
                {
                    currentTarget = null;
                    return;
                } else
                {
                    if (npcList.Count > 0) //To ensure that the list actually exists. Just in case.
                        {
                            currentTarget = npcList[npcList.IndexOf(currentTarget) + 1];
                        } else 
                        {
                            currentTarget = null;
                        }
                }
            }

            Vector2 direction = currentTarget.Center - Projectile.Center;
            float speed = 12f;

            direction.Normalize();
            Vector2 desiredVelocity = direction * speed;

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.15f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 4; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 100, default, .5f);
                Main.dust[d].position = Projectile.Center;
            }
        }
    }
}
