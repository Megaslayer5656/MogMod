using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MogMod.NPCs.Global;

namespace MogMod.Common
{
    public class ProjectileModifications : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool applyBuffOnHit;
        public bool sayTimesHitOnThirdHit;
        private Color trailColor;
        private bool trailActive;
        public static LocalizedText HitMessage { get; private set; }

        public void SetTrail(Color color)
        {
            trailColor = color;
            trailActive = true;
        }

        public override void SetStaticDefaults()
        {
            HitMessage = Mod.GetLocalization($"{nameof(ProjectileModifications)}.HitMessage");
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (sayTimesHitOnThirdHit)
            {
                ProjectileModificationGlobalNPC globalNPC = target.GetGlobalNPC<ProjectileModificationGlobalNPC>();
                if (globalNPC.timesHitByModifiedProjectiles % 3 == 0)
                {
                    Main.NewText(HitMessage.Format(globalNPC.timesHitByModifiedProjectiles));
                }
                target.GetGlobalNPC<ProjectileModificationGlobalNPC>().timesHitByModifiedProjectiles += 1;
            }

            if (applyBuffOnHit)
            {
                target.AddBuff(BuffID.OnFire, 50);
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (trailActive)
            {
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.TintableDustLighted, default, default, default, trailColor);
            }
        }
    }
}
