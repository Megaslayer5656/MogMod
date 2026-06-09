using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class ScintillantStabber : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static int hitCount = 0;
        public override void SetDefaults()
        {
            Item.width = Item.height = 36;

            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 6;
            Item.shoot = ModContent.ProjectileType<ScintillantStabberProj>();
            Item.shootSpeed = 2f;
            Item.knockBack = 3f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Pink;
            Item.value = MogGlobalItem.RarityPinkBuyPrice;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.7f }, player.Center);
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Gladius).
                AddIngredient(ItemID.Amber, 8).
                AddIngredient(ItemID.AncientCloth, 2).
                AddIngredient(3783).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}