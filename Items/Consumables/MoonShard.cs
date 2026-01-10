using MogMod.Common.Player;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Items.Other;

namespace MogMod.Items.Consumables
{
    public class MoonShard : ModItem
    {
        public static readonly SoundStyle shardUse = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/MoonShardConsume")
        {
            Volume = 1.1f,
            PitchVariance = .2f,
            MaxInstances = 3
        };
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.buffType = ModContent.BuffType<Buffs.PotionBuffs.MoonShardBuff>();
            Item.buffTime = 36000;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(shardUse, player.Center);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.LunarTabletFragment, 3).
                AddIngredient(ItemID.Moonglow, 3).
                AddIngredient(ItemID.Ectoplasm, 2).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}