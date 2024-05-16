using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Consumables
{
    public class ETGC : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 5;
            Item.height = 5;
            Item.useStyle = ItemUseStyleID.DrinkLong;
            Item.useAnimation = 25;
            Item.useTime = 10;
            Item.useTurn = true;
            Item.UseSound = Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/Inject")
            {
                Volume = 0.9f,
                PitchVariance = 0.2f,
                MaxInstances = 3,
            };

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.rare = ItemRarityID.Green;
            Item.value = 10000;
            Item.buffType = ModContent.BuffType<Buffs.ETGCbuff>(); // Specify an existing buff to be applied when used.
            Item.buffTime = 3 * 60;
        }
    }
}