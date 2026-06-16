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
    public class SylvanStiletto : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 36;

            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 12;
            Item.shoot = ModContent.ProjectileType<SylvanStilettoProj>();
            Item.shootSpeed = 3f;
            Item.knockBack = 4.5f;

            Item.noMelee = true;
            Item.channel = true;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 11;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.useStyle = ItemUseStyleID.Shoot;
            SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 0.5f }, player.Center);
            Projectile.NewProjectile(source, position, velocity * 0.75f, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.JungleSpores, 12).
                AddIngredient(ItemID.Stinger, 10).
                AddIngredient(ItemID.BeeWax, 8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}