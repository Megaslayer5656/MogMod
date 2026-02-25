using Microsoft.Xna.Framework;
using MogMod.Buffs.PotionBuffs;
using MogMod.Common.MogModPlayer;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectile : GlobalProjectile
    {
        // exists for projectile utils hopefully

        public bool CanSplit = true;
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
        #endregion
    }
}