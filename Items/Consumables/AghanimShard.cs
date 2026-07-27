using MogMod.Buffs.PotionBuffs;
using MogMod.Items.Other;
using MogMod.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Consumables
{
    public class AghanimShard : ModItem //, ILocalizedModType
    {
        public const int ManaBoost = 100;
        public const float MagicDamageBoost = 0.20f;
        public const int ManaRegenBoost = 40;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ManaBoost, MagicDamageBoost.ToPercent(), ManaRegenBoost.ToRegenPerSecond());
        //public new string LocalizationCategory => "Items.Misc";
        public static readonly SoundStyle aghanimUse = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/AghanimShard")
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
            Item.width = Item.height = 32;
            Item.useAnimation = Item.useTime = 15;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.buffType = ModContent.BuffType<AghanimShardBuff>(); // Specify an existing buff to be applied when used.
            Item.buffTime = 36000; // The amount of time the buff declared in Item.buffType will last in ticks. 36000 / 60 is 600, so this buff will last 10 minutes.
        }
        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(aghanimUse, player.Center);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe(5).
                DisableDecraft().
                AddIngredient(ItemID.MagicPowerPotion, 1).
                AddIngredient(ItemID.ManaRegenerationPotion, 1).
                AddIngredient(ItemID.Ectoplasm, 2).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.CrystalBall).
                Register();
        }
    }
}