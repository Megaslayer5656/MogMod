using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Items.Weapons.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class SangeAndYasha : ModItem, ILocalizedModType
    {
        // delete this item
        public new string LocalizationCategory => "Items.Accessories";
        public const float sizeMult = 1.5f;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Red;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // increase size of melee weapons
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSange = true;

            player.GetAttackSpeed(DamageClass.Melee) += .24f;
            player.statLifeMax2 += 50;
            player.lifeRegen += 6;
            player.GetDamage(DamageClass.Generic) += .16f;
            player.accRunSpeed += player.accRunSpeed * .24f;
            player.lifeSteal *= 1.5f;
        }
        public static float SangeWeaponSize(MogPlayer mogPlayer)
        {
            return sizeMult;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Sange>(1).
                AddIngredient<Yasha>(1).
                AddIngredient(ItemID.AnkhShield, 1).
                AddIngredient<GriefBar>(7).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
