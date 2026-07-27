using Microsoft.Xna.Framework;
using MogMod.Items.Weapons.Ranged;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public sealed class BloodGrenadeProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 14;
            Projectile.height = 20;
            Projectile.friendly = true; // Can hit enemies
            Projectile.hostile = false; // can hit you
            Projectile.penetrate = 1; // Number of enemies it can hit before disappearing (1 for explosion on contact)
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile; // determines what ai it uses
            Projectile.timeLeft = 300; // time before explosion

            MogModGlobalProjectile mogProj = Projectile.MogMod();
            mogProj.bloodDamage = BloodGrenade.BloodDamage;
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
        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 130;
                Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
                Projectile.localAI[1] = -1f;
                Projectile.maxPenetrate = 0;
                Projectile.Damage();
            }
            for (int i = 0; i < 20; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, default(Color), 1f);
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 0, default(Color), 1f);
            Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 180);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Slow, 180);
        }
    }
}