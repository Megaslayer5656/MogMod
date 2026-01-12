using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace MogMod.Buffs.PotionBuffs
{
    public class DragonInstallBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statLifeMax2 += 100;
            player.lifeRegen += 5;
            player.statDefense += 20;
            player.GetAttackSpeed(DamageClass.Melee) += .5f;
            dragonInstallDusts(player);
        }

        private void dragonInstallDusts(Player player)
        {
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(player.Center, 1, 1, DustID.CrimsonTorch);
                Main.dust[d].noLight = false;
                Main.dust[d].scale = 1.25f;
            }
        }
    }
}