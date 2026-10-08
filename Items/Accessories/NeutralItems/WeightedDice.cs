using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.NPCs.Global;
using MogMod.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace MogMod.Items.Accessories.NeutralItems
{
    public class WeightedDice : NeutralItem
    {
        public const int RandDamage = 7;
        public const float RandGold = 0.3f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(RandDamage, RandGold.ToPercent());
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 30;
            Item.height = 26;
            Item.rare = ItemRarityID.Green;
            Item.value = MogGlobalItem.RarityGreenBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingWeightedDice = true;
        }
        public static int WeightedDiceDamageEffect()
        {
            int damage = Main.rand.Next(-RandDamage, RandDamage + 1);
            return damage;
        }
        public static void WeightedDiceGoldEffect(NPC target)
        {
            MogModGlobalNPC mogNPC = target.MogMod();
            if (mogNPC.weightedDiceEffect) return;
            float gold = Main.rand.NextFloat(-RandGold, RandGold);
            if (gold < 0f) gold = 0f;
            target.value *= (1f + gold);
            mogNPC.weightedDiceEffect = true;
            target.netUpdate = true;
        }
    }
}