using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Rarities;
using MogMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories.NeutralItems.Aspects
{
    public class NoxiousAspect : NeutralItem, IHoldShiftTooltipItem
    {
        public const int DamageMin = 10;
        public const int DamageMax = 51;
        public static Color Colour = new(219f, 47f, 237f);
        public bool HidesNormalTooltip => true;
        public Color? TooltipExtensionColor => new(219, 47, 237);
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
        }
        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.005f, 0.015f);
            Lighting.AddLight(Item.Center, 219f * brightness, 47f * brightness, 237f * brightness);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MogPlayer mogPlayer = player.MogMod();
            mogPlayer.wearingToxic = true;
            mogPlayer.toxicVisual = !hideVisual;
        }
    }
}