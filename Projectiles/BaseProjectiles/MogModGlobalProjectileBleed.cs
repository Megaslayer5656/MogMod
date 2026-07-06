using MogMod.Common.MogModPlayer;
using MogMod.Items.Weapons.Melee;
using MogMod.NPCs.Global;
using MogMod.Projectiles.Classless;
using MogMod.Projectiles.MagicProjectiles;
using MogMod.Projectiles.Melee;
using MogMod.Projectiles.RangedProjectiles;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectileBleed : GlobalProjectile
    {
        public int bloodDamage;
        public override void SetDefaults(Projectile entity)
        {
            if (entity.type == ModContent.ProjectileType<BloodMagicProjectile>())
                bloodDamage = 17;
            else if (entity.type == ModContent.ProjectileType<RiversOfBloodProj>())
                bloodDamage = 300;
            else if (entity.type == ModContent.ProjectileType<SplinterProjectile>())
                bloodDamage = 10;
            else if (entity.type == ModContent.ProjectileType<BloodGrenadeProjectile>())
                bloodDamage = 14;
            else if (entity.type == ModContent.ProjectileType<APLapua>())
                bloodDamage = 200;
            else if (entity.type == ModContent.ProjectileType<BloodthornBeam>())
                bloodDamage = 20; //This one in particular might need some adjustings
            else if (entity.type == ModContent.ProjectileType<LordOfBloodsSpearProj>()) //The spear itself
                bloodDamage = 120;
            else if (entity.type == ModContent.ProjectileType<LordOfBloodsSpearBloodProj>()) //The huge aoe one
                bloodDamage = 250;
            else if (entity.type == ModContent.ProjectileType<BloodExplosion>())
                bloodDamage = 500;
            else
                bloodDamage = 0;
        }

        public override void AI(Projectile projectile)
        {
            if (bloodDamage > 1)
            {
                projectile.netUpdate = true; //May have to remove if causes lag
                projectile.extraUpdates = 1;
            }
        }

        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            if (bloodDamage > 0)
            {
                binaryWriter.Write(bloodDamage);
            }
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            bloodDamage = binaryReader.ReadInt32();
        }


        public override bool InstancePerEntity => true;
    }
}
