using MogMod.Buffs.PotionBuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Common.MogModPlayer;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectile : GlobalProjectile
    {
        // exists for projectile utils hopefully
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
    }
}