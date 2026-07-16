using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace MogMod.Items.Armor.Seraphic
{
    [AutoloadEquip(EquipType.Body)]
    public class SeraphicBreastplate : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public const int ReviveDuration = 300;
        public const int ReviveCooldown = 18000;
        public const int MeleeCritBoost = 32;
        public const float WhipSpeedBoost = 0.2f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MeleeCritBoost, WhipSpeedBoost.ToPercent());
        public int equipBack = -1;
        public override void Load()
        {
            // All code below runs only if we're not loading on a server
            if (Main.netMode != NetmodeID.Server)
            {
                // Add equip textures
                equipBack = EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this);
            }
        }
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body);

            ArmorIDs.Body.Sets.IncludedCapeBack[Item.bodySlot] = equipBack;
            ArmorIDs.Body.Sets.IncludedCapeBackFemale[Item.bodySlot] = equipBack;

            ArmorIDs.Body.Sets.HidesTopSkin[equipSlot] = true;
            ArmorIDs.Body.Sets.HidesArms[equipSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;

            Item.defense = 34;

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance<MeleeDamageClass>() += MeleeCritBoost;
            player.GetAttackSpeed<SummonMeleeSpeedDamageClass>() += WhipSpeedBoost;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (Main.LocalPlayer.armor[0].type == ItemType<SeraphicHelm>())
                SeraphicHelm.ModifySetTooltips(this, tooltips);
            else if (Main.LocalPlayer.armor[0].type == ItemType<SeraphicCrown>())
                SeraphicCrown.ModifySetTooltips(this, tooltips);
        }
        // recipe will be changed eventually
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HallowedPlateMail).
                AddIngredient(ItemID.LunarBar, 16).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.AncientHallowedPlateMail).
                AddIngredient(ItemID.LunarBar, 16).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}