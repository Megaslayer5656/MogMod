using MogMod.Common.MogModPlayer;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class WraithBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wraithActive = true;

            if (player.buffTime[buffIndex] < 1)
            {
                player.statLife = 0;
                player.Hurt(PlayerDeathReason.ByCustomReason($"{player.name} soul faded away..."), player.statLifeMax2, 0);
                mogPlayer.doUndying();
            }

            int d = Dust.NewDust(player.position, player.width, player.height, DustID.Terra, 0, 0, 100, default, 1f);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity.Y -= 1.5f;
            Main.dust[d].scale = 0.8f;
            Main.dust[d].fadeIn = Main.rand.NextFloat(0.6f, 0.8f);
        }
    }
}