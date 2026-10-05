using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic
{
    public class Khanda : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public static Color WeakColor => Color.Orchid;
        public static Color StrongColor => Color.Fuchsia;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 46;

            Item.damage = 40;
            Item.crit = 14;
            Item.ArmorPenetration = 10;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useTime = Item.useAnimation = 20;
            Item.knockBack = 0f;
            Item.UseSound = SoundID.Item20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<KhandaHoldout>();
            Item.shootSpeed = 2f;

            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Crystalys>().
                AddIngredient<Phylactery>().
                AddRecipeGroup("AnyCobaltBar", 12).
                AddIngredient(ItemID.LightShard).
                AddIngredient<PointBooster>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}