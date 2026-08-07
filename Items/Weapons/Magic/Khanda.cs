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
        public override void SetStaticDefaults() => Item.staff[Item.type] = true;
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 46;
            Item.damage = 22;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item13;
            Item.noMelee = true;
            Item.noUseGraphic = false;
            Item.channel = true;
            Item.knockBack = 0f;
            Item.shoot = ModContent.ProjectileType<KhandaBeam>();
            Item.shootSpeed = 30f;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
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