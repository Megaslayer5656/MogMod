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

namespace MogMod.Projectiles.MeleeProjectiles
{
    public class MarkerTargetProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.MeleeProjectiles";
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow; //Gonna have to change this
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1f;

            AIType = ProjectileID.Bullet; //Gonna have to change this too
        }

        public override void AI()
        {
            //TODO: Make it move away from npc and slow down
        }

        public override void OnKill(int timeLeft)
        {
            //TODO: Make vfx
        }

        public override bool CanHitPlayer(Player target)
        {
            if (target.whoAmI == Projectile.owner || Projectile.owner == 255)
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
            Projectile.timeLeft = 1;
            //TODO: Spawn another projectile that homes on the target that was initially hit by the projectile
        }
    }
}