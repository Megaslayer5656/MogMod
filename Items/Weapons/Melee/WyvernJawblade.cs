using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.BaseProjectiles;
using MogMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class WyvernJawblade : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override int ProjectileType => ModContent.ProjectileType<WyvernJawbladeHoldout>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 80;
            Item.height = 88;

            Item.damage = 38;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 60;
            Item.channel = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item1 with { Pitch = -0.1f };

            Item.value = MogGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FossilOre, 20).
                AddIngredient(ItemID.Leather, 5).
                AddIngredient<CraftingRecipe>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}