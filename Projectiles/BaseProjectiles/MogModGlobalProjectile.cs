using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectile : GlobalProjectile
    {
        // exists for projectile utils hopefully
        private Random random = new Random();
        public NPC.HitInfo hitInfo;
        public bool CanSplit = true;
        public bool radiantProc = false;
        public bool gunpowderProc = false;
        public bool shivProc = false;

        public int gunpowderCap = 30;
        public int shivCap = 350;
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }
        // Amount of extra updates that are set in SetDefaults.
        public int defExtraUpdates = -1;

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                MogPlayer modPlayer = target.GetModPlayer<MogPlayer>();
                modPlayer.doParry(target, target.Center);
                modifiers.Cancel();

                projectile.velocity.X = projectile.velocity.X * -1;
                projectile.velocity.Y = projectile.velocity.Y * -1;
                projectile.friendly = true;
                projectile.hostile = false;
                projectile.damage *= 5;
            }
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer modPlayer = player.MogMod();
            var source = player.GetSource_OnHit(target);
            int itemDamage = player.HeldItem.damage;
            int enemyMaxHP = target.lifeMax;
            int shivDamage = 0;

            if (itemDamage <= 40)
                gunpowderCap = itemDamage;
            else
                gunpowderCap = 40;
            if (Convert.ToInt32(enemyMaxHP * 0.01) <= shivCap)
                shivDamage = Convert.ToInt32(enemyMaxHP * 0.01) + 50;
            else
                shivDamage = shivCap;

            radiantProc = random.Next(2) == 0;
            gunpowderProc = random.Next(5) == 0;

            if (radiantProc)
            {
                if (modPlayer.wearingRadiantArmor && projectile.type != ProjectileType<RadiantBeamProj>() && projectile.DamageType == DamageClass.Magic)
                {
                    MogModUtils.ProjectileRain(source, target.Center, 100f, 50f, 1500f, 1500f, 10f, ModContent.ProjectileType<RadiantBeamProj>(), Convert.ToInt32(projectile.damage / .75f), projectile.knockBack, projectile.owner);
                }
            }

            if (gunpowderProc)
            {
                if (modPlayer.wearingGunpowderGauntlet && projectile.type != ProjectileType<GunpowderProj>() && projectile.DamageType == DamageClass.Magic)
                {
                    int gunpowderProc = Projectile.NewProjectile(source, target.Center, new Vector2(10f, 10f), ProjectileType<GunpowderProj>(), gunpowderCap, 0f, projectile.owner);
                }
            }

            shivProc = random.Next(5) == 0;
            if (shivProc && modPlayer.wearingSerratedShiv)
            {
                hitInfo = new NPC.HitInfo
                {
                    Damage = shivDamage,
                    Knockback = 0,
                    HitDirection = 0,
                    Crit = false,
                    DamageType = DamageClass.Default
                };
                target.StrikeNPC(hitInfo);
                NetMessage.SendStrikeNPC(target, hitInfo);
                Rectangle r = new Rectangle((int)target.position.X, (int)target.position.Y - 50, target.width, target.height);
                Color textColor = new Color(210, 180, 140);
                CombatText.NewText(r, textColor, "Strike!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.TrueStrikeProcTextSync);
                    packet.Write(target.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
                doTrueStrikeFX(target.Center);
            }
        }

        // stolen STRAIGHT from fargos souls mod
        // makes fishing rods spawn more bobbers when wearing certain accessories
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];
            MogPlayer modPlayer = player.MogMod();

            if (projectile.bobber && CanSplit && source is EntitySource_ItemUse)
            {
                int splitCount = 0;
                if (modPlayer.wearingFishSlop1)
                    splitCount += 10;
                if (modPlayer.wearingFishSlop2)
                    splitCount += 15;
                if (player.whoAmI == Main.myPlayer && splitCount > 0)
                    SplitProj(projectile, splitCount, MathHelper.Pi / 3, 1);
            }
        }

        #region slop
        public static List<Projectile> SplitProj(Projectile projectile, int number, float maxSpread, float damageRatio, bool allowMoreSplit = false)
        {
            //if its odd, we just keep the original 
            if (number % 2 != 0)
            {
                number--;
            }
            List<Projectile> projList = [];
            Projectile split;
            double spread = maxSpread / number;
            for (int i = 0; i < number / 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int factor = j == 0 ? 1 : -1;
                    split = NewProjectileDirectSafe(projectile.GetSource_FromThis(), projectile.Center, projectile.velocity.RotatedBy(factor * spread * (i + 1)), projectile.type, (int)(projectile.damage * damageRatio), projectile.knockBack, projectile.owner, projectile.ai[0], projectile.ai[1]);
                    if (split != null)
                    {
                        split.ai[2] = projectile.ai[2];
                        split.localAI[0] = projectile.localAI[0];
                        split.localAI[1] = projectile.localAI[1];
                        split.localAI[2] = projectile.localAI[2];

                        split.friendly = projectile.friendly;
                        split.hostile = projectile.hostile;
                        split.timeLeft = projectile.timeLeft;
                        split.DamageType = projectile.DamageType;
                        projList.Add(split);
                    }
                }
            }

            return projList;
        }
        public static Projectile NewProjectileDirectSafe(IEntitySource spawnSource, Vector2 pos, Vector2 vel, int type, int damage, float knockback, int owner = 255, float ai0 = 0f, float ai1 = 0f)
        {
            int p = Projectile.NewProjectile(spawnSource, pos, vel, type, damage, knockback, owner, ai0, ai1);
            return p < Main.maxProjectiles ? Main.projectile[p] : null;
        }
        public static void doTrueStrikeFX(Vector2 position)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath56, position);
            for (int i = 0; i < 40; i++)
            {
                int strike = Dust.NewDust(position, 20, 20, DustID.CopperCoin, 0, 0, 100, default, 2f);
                Main.dust[strike].velocity.Y *= 1.05f;
                Main.dust[strike].noGravity = true;
            }
        }
        #endregion
    }
}