using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Consumables
{
    public class Glue : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 5;
            Item.height = 5;
            Item.useStyle = ItemUseStyleID.DrinkLong;
            Item.useAnimation = 60;
            Item.useTime = 60;
            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
            Item.value = 10000;
            Item.buffType = ModContent.BuffType<Buffs.GlueBuff>();
            Item.buffTime = 5 * 60;
        }
        public override void UseItemFrame(Player player)
        {
            player.AddBuff(ModContent.BuffType<Buffs.GlueDebuff>(), 10000 * 60);
        }
    }
}