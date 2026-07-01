using Microsoft.Xna.Framework;
using MogMod.Common.Systems;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Buffs.PotionBuffs
{
    public class SeraphicReviveBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            foreach (int debuff in player.buffType)
            {
                if (MogModBuffSets.IsDebuff[debuff])
                    player.buffImmune[debuff] = true;
            }
            Dust seraphic = Dust.NewDustDirect(player.position, player.width, player.height, Main.rand.NextBool(3) ? DustID.HallowSpray : 133, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.8f);
            seraphic.position.X += (float)Main.rand.Next(-20, 21);
            seraphic.position.Y += (float)Main.rand.Next(-20, 21);
            seraphic.velocity *= 0.9f;
            seraphic.noGravity = true;
            seraphic.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
            seraphic.shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
            if (Main.rand.NextBool())
                seraphic.scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
        }
    }
}