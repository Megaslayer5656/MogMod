using Microsoft.Xna.Framework;
using MogMod.Buffs.Pets;
using MogMod.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Pets
{
    // made by my sister
    public class Gingy : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Pets";
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<GingyPet>(), ModContent.BuffType<GingyBuff>());
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Orange;
        }
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
                player.AddBuff(Item.buffType, 3600, true);
        }
    }
}