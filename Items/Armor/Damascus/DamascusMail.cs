using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Damascus
{

    [AutoloadEquip(EquipType.Body)]
    public class DamascusMail : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int CritBoost = 6;
        public const float MeleeSpeedBoost = 0.06f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(CritBoost, MeleeSpeedBoost.ToPercent());
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
            Item.width = 34;
            Item.height = 24;
            Item.defense = 7;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.armor[0].type == ItemType<DamascusHelm>())
                DamascusHelm.ModifySetTooltips(this, tooltips);
            else if (Main.LocalPlayer.armor[0].type == ItemType<DamascusMask>())
                DamascusMask.ModifySetTooltips(this, tooltips);
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritBoost;
            player.GetAttackSpeed<MeleeDamageClass>() += MeleeSpeedBoost;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<FuciumBar>(15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}