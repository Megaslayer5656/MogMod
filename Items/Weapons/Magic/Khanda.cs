using MogMod.Items.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Items.Weapons.Melee;
using MogMod.Projectiles.MagicProjectiles;

namespace MogMod.Items.Weapons.Magic
{
    public class Khanda : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
        }
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 46;
            Item.damage = 21;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 8;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item13;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.shoot = ModContent.ProjectileType<KhandaBeam>();
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.Purple;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Crystalys>(1).
                AddIngredient<Phylactery>(1).
                AddRecipeGroup("CobaltBar", 12).
                AddIngredient(ItemID.LightShard, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}