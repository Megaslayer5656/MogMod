using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.Tigla
{
    [AutoloadEquip(EquipType.Head)]
    public class TiglaHelmet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            // so the players hair can be seen with the armor equipped
            ArmorIDs.Head.Sets.DrawHatHair[equipSlot] = true;

            // set bonus text
            SetBonusText = this.GetLocalization("SetBonus");
        }
        public override void SetDefaults()
        {
            Item.width = Item.height = 24;
            Item.defense = 16;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<TiglaVest>() && legs.type == ModContent.ItemType<TiglaPants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            // set bonus description
            player.setBonus = SetBonusText.Value;

            // hunter and ammo potion effects
            player.detectCreature = true;
            player.ammoPotion = true;

            // knockback immunity
            player.noKnockback = true;

            // rifle scope effect
            player.scope = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.2f;
            player.GetCritChance<RangedDamageClass>() += 0.08f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.UltrabrightHelmet, 1).
                AddIngredient(ItemID.ShroomiteMask, 1).
                AddIngredient(ItemID.Cog, 100).
                AddIngredient<UltimateOrb>(3).
                AddIngredient(ItemID.SniperScope, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.UltrabrightHelmet, 1).
                AddIngredient(ItemID.ShroomiteHeadgear, 1).
                AddIngredient(ItemID.Cog, 100).
                AddIngredient<UltimateOrb>(3).
                AddIngredient(ItemID.SniperScope, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.UltrabrightHelmet, 1).
                AddIngredient(ItemID.ShroomiteHelmet, 1).
                AddIngredient(ItemID.Cog, 100).
                AddIngredient<UltimateOrb>(3).
                AddIngredient(ItemID.SniperScope, 1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}