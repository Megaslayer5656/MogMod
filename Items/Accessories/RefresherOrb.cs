using MogMod.Common.Player;
using MogMod.Common.Systems;
using MogMod.Utilities;
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
    public class RefresherOrb : ModItem
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.RefresherOrbKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 50;
            player.GetDamage(DamageClass.Magic) += .10f;
            player.GetDamage(DamageClass.Summon) += .10f;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingRefresherOrb = true;
        }
    }
}
