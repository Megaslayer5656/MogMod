using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Hellfire
{
    [AutoloadEquip(EquipType.Body)]
    public class HellfireBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const float DamageBoost = 0.1f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost.ToPercent());
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 18;

            Item.defense = 19;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips) => HellfireMask.ModifySetTooltips(this, tooltips);
        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += DamageBoost;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MoltenBreastplate).
                AddIngredient<HellfireBar>(15).
                AddIngredient<ScorchedCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}