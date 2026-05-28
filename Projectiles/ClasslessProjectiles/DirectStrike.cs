﻿using Microsoft.Xna.Framework;
using MogMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Projectiles.ClasslessProjectiles
{
    public class DirectStrike : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public bool invalidTarget => (Projectile.ai[0] < 0f || Projectile.ai[0] > 199f);

        // You can set Projectile AI 1 & 2 to the X/Y velocity that you would like to launch the target, even if the direct strike deals no damage.
        // This lets Direct Strikes be used as "Direct Nudges" which deal no damage but push something around.
        public Vector2 pushVelocity => new(Projectile.ai[1], Projectile.ai[2]);

        // If you set Knockback to anything below zero, the custom knockback will be able to effect enemies that normally ignore knockback
        public bool hasStongDisplacement = false;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
        }

        public override void AI()
        {
            if (Projectile.knockBack < 0)
            {
                hasStongDisplacement = true;
                Projectile.knockBack = 0;
            }

            // If the target is moving VERY fast, direct strikes spawned on top of them can actually miss
            // Setting a target will guarantee hits on said target by teleporting the projectile onto their center every frame
            // Setting a target will guarantee hits on said target by teleporting the projectile onto them every frame
            if (!invalidTarget)
                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;
        }

        // If the AI parameter isn't a valid NPC slot, it can hit anything. Otherwise it can only hit one NPC.
        public override bool? CanHitNPC(NPC target)
        {
            if (invalidTarget || Projectile.ai[0] == target.whoAmI)
                return null;
            return (bool?)false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                NPC target = Main.npc[(int)Projectile.ai[0]];
                if (pushVelocity != Vector2.Zero && pushVelocity.X < 255f && !invalidTarget && target.CanBeMoved(hasStongDisplacement))
                {
                    target.velocity = (pushVelocity * (target.knockBackResist == 0 ? 0.5f : 1));
                }
                return true;
            }
            return false;
        }
    }
}   