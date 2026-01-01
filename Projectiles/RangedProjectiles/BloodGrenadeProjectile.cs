using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace MogMod.Projectiles.RangedProjectiles
{
    public sealed class BloodGrenadeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles";
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Blood Grenade"); // might not actually do anything
        }
        public override void SetDefaults()
        {
            //Projectile.damage = 90; // might do something
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true; // Can hit enemies
            Projectile.hostile = false; // can hit you
            Projectile.penetrate = 1; // Number of enemies it can hit before disappearing (1 for explosion on contact)
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile; // determines what ai it uses
            Projectile.timeLeft = 100; // time before explosion
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // explosions do less damage to Eater of Worlds in expert mode
            if (Main.expertMode)
            {
                if (target.type >= NPCID.EaterofWorldsHead && target.type <= NPCID.EaterofWorldsTail)
                {
                    modifiers.FinalDamage /= 5;
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 0, default(Color), 1f);
            }
            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 300);
            target.AddBuff(BuffID.Poisoned, 300);

            // TODO: make explode in aoe radius
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Slow, 200);
            target.AddBuff(BuffID.Poisoned, 200);
        }
    }
}