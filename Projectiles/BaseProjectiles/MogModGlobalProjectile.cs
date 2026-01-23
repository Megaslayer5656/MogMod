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

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (target.HasBuff(ModContent.BuffType<Parrying>()))
            {
                info = new Player.HurtInfo
                {
                    Damage = 1,
                    Knockback = 0,
                    HitDirection = 0,
                    Dodgeable = false,
                    SoundDisabled = true
                };
                projectile.velocity.X = projectile.velocity.X * -1;
                projectile.velocity.Y = projectile.velocity.Y * -1;
                projectile.friendly = true;
                projectile.hostile = false;
                projectile.damage *= 5;
            }
        }
    }
}
