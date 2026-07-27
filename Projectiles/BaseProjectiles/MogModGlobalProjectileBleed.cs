using MogMod.Items.Global;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MogMod.Projectiles.BaseProjectiles
{
    public partial class MogModGlobalProjectile : GlobalProjectile
    {
        /// <summary> How much blood damage this projectile does. Higher values lead to faster blood procs. </summary>
        /// <remarks> This value does not apply a tooltip to an item, instead use <see cref="MogGlobalItem.visualBloodDamage"/> to modify the items tooltip.</remarks>
        public int bloodDamage = 0;
        public void BloodAI(Projectile projectile)
        {
            if (bloodDamage > 1)
            {
                projectile.netUpdate = true; //May have to remove if causes lag
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
    }
}