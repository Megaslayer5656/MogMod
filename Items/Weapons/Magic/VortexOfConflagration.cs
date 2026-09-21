using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class VortexOfConflagration : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;

            Item.damage = 85;
            Item.mana = 4;
            Item.DamageType = DamageClass.Magic;
            Item.useTime = Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<VoCProj>();
            Item.shootSpeed = 2f;

            Item.rare = ItemRarityID.Red;
            Item.value = MogGlobalItem.RarityRedBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.RazorbladeTyphoon).
                AddIngredient<InfernoMaelstrom>().
                AddIngredient<BrinyRind>(12).
                AddIngredient(ItemID.FragmentVortex, 8).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}