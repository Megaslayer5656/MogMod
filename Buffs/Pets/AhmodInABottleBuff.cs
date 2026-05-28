using Microsoft.Xna.Framework;
using MogMod.Projectiles.Pets;
using MogMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Buffs.Pets
{
    internal class AhmodInABottleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;
            player.MogMod().ahmodPet = true;
            bool PetProjectileNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<AhmodInABottlePet>()] <= 0;
            if (PetProjectileNotSpawned && player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(player.GetSource_Buff(buffIndex), player.Center, Vector2.Zero, ModContent.ProjectileType<AhmodInABottlePet>(), 0, 0f, player.whoAmI, 50f, 0f);
        }
    }
}