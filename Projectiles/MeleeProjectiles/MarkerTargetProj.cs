using Microsoft.Xna.Framework;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using MogMod.NPCs.Global;
using Microsoft.CodeAnalysis;
using MogMod.Common.MogModPlayer;
using Microsoft.Build.Evaluation;
using MogMod.Projectiles.MagicProjectiles;

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class MarkerTargetProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        Random rand = new Random();
        public bool canHit = false;
        public bool hasHit = false;
        NPC marked;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 37;
            Projectile.aiStyle = ProjAIStyleID.Arrow; //Gonna have to change this later
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 2f;

            AIType = ProjectileID.Bullet; //Gonna have to change this too
        }

        public override void AI() //This projectile is a little janky so I'll fix it later
        {

            Projectile.ai[1]++;

            if (Projectile.ai[1] > 30)
            {
                canHit = true;
            }

            Projectile.velocity *= 0.975f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 12) // This will change the sprite every 8 frames (0.13 seconds). Feel free to experiment.
            {
                Projectile.frame++;
                Projectile.frame %= 4; // Will reset to the first frame if you've gone through them all.
                Projectile.frameCounter = 0;
            }

            if (Projectile.timeLeft == 5)
            {
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (!npc.active)
                        continue;

                    if (!npc.TryGetGlobalNPC<MogModGlobalNPC>(out var globalNPC))
                        continue;

                    if (globalNPC.markedByMarker)
                    {
                        globalNPC.markedByMarker = false;
                    }
                }

                
            }
        }

        public override void OnKill(int timeLeft)
        {
            int player = Projectile.owner;
            MogPlayer mogPlayer = Main.player[player].GetModPlayer<MogPlayer>();
            mogPlayer.markerProjOut = false;

            for (int i = 0; i < 20; i++)
            {
                float randSpeed = rand.Next(-5, 5);
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.IchorTorch, randSpeed, randSpeed, 0, default, 1f);
                Main.dust[d].noGravity = true;
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            if ((target.whoAmI == Projectile.owner || Projectile.owner == 255) && canHit)
            {
                return true;
            } else
            {
                return false;
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Cancel();
            Projectile.Kill();
            Vector2 velocity = new Vector2(1f, 1f);
            if (!hasHit)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 rotatedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(360));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, rotatedVelocity, ModContent.ProjectileType<MarkerProjProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                hasHit = true;
            }
        }
    }
}