using MogMod.Common.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ArmletOfMordiggian : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statDefense += 7;
            player.GetAttackSpeed(DamageClass.Generic) += .20f;
            player.GetDamage(DamageClass.Generic) += .15f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.armletActive = true;
        }
    }
}
