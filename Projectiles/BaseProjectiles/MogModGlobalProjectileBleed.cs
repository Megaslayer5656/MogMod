using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.NPCs.Global;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectileBleed : GlobalProjectile
    {
        public int bloodDamage;

        public override void SetDefaults(Projectile entity)
        {
            if (entity.type == ModContent.ProjectileType<BloodMagicProjectile>())
            {
                bloodDamage = 17;
            } 
            else if (entity.type == ModContent.ProjectileType<RiversOfBloodProj>())
            {
                bloodDamage = 300;
            }
            else if (entity.type == ModContent.ProjectileType<SplinterProjectile>())
            {
                bloodDamage = 10;
            }
            else if (entity.type == ModContent.ProjectileType<BloodGrenadeProjectile>())
            {
                bloodDamage = 14;
            }
            else if (entity.type == ModContent.ProjectileType<APLapua>())
            {
                bloodDamage = 200;
            }
            else
            {
                bloodDamage = 0;
            }
        }

        public override void AI(Projectile projectile)
        {

            int player = projectile.owner;

            MogPlayer mogPlayer = Main.player[player].GetModPlayer<MogPlayer>();
            projectile.localAI[0] = bloodDamage * (mogPlayer.exultationEquipped ? 1.15f : 1f);
            

            if (bloodDamage > 1)
            {
                projectile.netUpdate = true; //May have to remove if causes lag
                projectile.extraUpdates = 1;
            }
        }

        public override bool InstancePerEntity => true;
    }
}
