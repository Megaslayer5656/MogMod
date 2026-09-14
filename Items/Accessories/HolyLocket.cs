using MogMod.Common.MogModPlayer;
using MogMod.Common.Systems;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class HolyLocket : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public const int MaxMinionBoost = 2;
        public const float MagicAndSummonDamageBoost = 0.07f;
        public const int ManaBoost = 70;
        public const int LifeHeal = 10; // shared with mana
        public const int ManaHeal = 10; // shared with life
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MaxMinionBoost, MagicAndSummonDamageBoost.ToPercent(), ManaBoost, LifeHeal);
        public override void ModifyTooltips(List<TooltipLine> list) => list.IntegrateHotkey(KeybindSystem.WandKeybind);
        ModKeybind keybindActive = null;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += MagicAndSummonDamageBoost;
            player.GetDamage(DamageClass.Summon) += MagicAndSummonDamageBoost;
            player.statManaMax2 += ManaBoost;
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.locketActive = true;
            float dim = .02f;
            Lighting.AddLight(player.Center, 75 * dim, 73 * dim, 61 * dim);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagicWand>().
                AddIngredient(ItemID.PygmyNecklace).
                AddIngredient<Diadem>().
                AddIngredient<SolRing>().
                AddRecipeGroup("AnyAdamantiteBar", 8).
                AddIngredient(ItemID.SoulofSight, 7).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}