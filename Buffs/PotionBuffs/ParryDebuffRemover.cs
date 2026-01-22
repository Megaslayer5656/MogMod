using MogMod.Buffs.Debuffs;
using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class ParryDebuffRemover : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.removeBuff(player, BuffID.OnFire);
            mogPlayer.removeBuff(player, BuffID.OnFire3);
            mogPlayer.removeBuff(player, BuffID.Frostburn);
            mogPlayer.removeBuff(player, BuffID.Frostburn2);
            mogPlayer.removeBuff(player, BuffID.Ichor);
            mogPlayer.removeBuff(player, BuffID.BrokenArmor);
            mogPlayer.removeBuff(player, BuffID.Webbed);
            mogPlayer.removeBuff(player, BuffID.Panic);
            mogPlayer.removeBuff(player, BuffID.Poisoned);
            mogPlayer.removeBuff(player, BuffID.CursedInferno);
            mogPlayer.removeBuff(player, BuffID.Confused);
            mogPlayer.removeBuff(player, BuffID.Bleeding);
            mogPlayer.removeBuff(player, BuffID.Oiled);
            mogPlayer.removeBuff(player, BuffID.ShadowFlame);
            mogPlayer.removeBuff(player, BuffID.Venom);
            mogPlayer.removeBuff(player, BuffID.Weak);
            mogPlayer.removeBuff(player, ModContent.BuffType<VonDebuff>());
        }
    }
}
