using MogMod.Common.MogModPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class SearingSignet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 36;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSearingSignet = true;

            player.GetDamage(DamageClass.Ranged) += .07f;
            player.GetDamage(DamageClass.Magic) += .07f;
            player.GetDamage(DamageClass.Summon) += .07f;
        }
    }
}
