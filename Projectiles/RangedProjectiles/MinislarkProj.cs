using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.RangedProjectiles
{
    public class MinislarkProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.RangedProjectiles";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 5; i++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[dust].scale = 0.5f;
                        Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item21, Projectile.position);
            }
            for (int j = 0; j < 2; j++)
            {
                float halfX = 0f;
                float halfY = 0f;
                if (j == 1)
                {
                    halfX = Projectile.velocity.X * 0.5f;
                    halfY = Projectile.velocity.Y * 0.5f;
                }
                int fishDust = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Water, 0f, 0f, 100, default, 1f);
                Main.dust[fishDust].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[fishDust].velocity *= 0.2f;
                Main.dust[fishDust].noGravity = true;
                fishDust = Dust.NewDust(new Vector2(Projectile.position.X + 3f + halfX, Projectile.position.Y + 3f + halfY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Water, 0f, 0f, 100, default, 0.5f);
                Main.dust[fishDust].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[fishDust].velocity *= 0.05f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (target.type != NPCID.TargetDummy)
            {
                player.AddBuff(ModContent.BuffType<EssenceShift>(), 180);
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                mogPlayer.essenceShiftLevel += 1;
                if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    mogPlayer.SyncEssenceShift(false);
                }
            }
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(344, 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<EssenceShift>(), 180);
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.essenceShiftLevel += 1;
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(344, 300);
            if (player.whoAmI == Main.myPlayer && Main.netMode == NetmodeID.MultiplayerClient)
            {
                mogPlayer.SyncEssenceShift(false);
            }
        }
    }
}