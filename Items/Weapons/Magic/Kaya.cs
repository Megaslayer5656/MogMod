using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Weapons.Magic
{
    public class Kaya : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        // CHANGE SPRITE TO WAND
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 30; // pretty sure 25 makes it do the same damage as sky fracture, the weapon it upgrades from
            Item.DamageType = DamageClass.Magic;
            Item.mana = 23;
            Item.useTime = 3;
            Item.useAnimation = 15;
            Item.reuseDelay = Item.useAnimation + 2;
            Item.useLimitPerAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 58, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<KayaProjectile>();
            Item.shootSpeed = 18f;
        }

        // here instead of SetDefaults since terraria doesnt like it being in there for some reason
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;

        // copy pasted from infernal rift in calamity
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float projSpeed = Item.shootSpeed;
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            float f = Main.rand.NextFloat() * MathHelper.TwoPi;
            float lowerLerpValue = 20f;
            float upperLerpValue = 60f;
            Vector2 projSpawnOffset = realPlayerPos + f.ToRotationVector2() * MathHelper.Lerp(lowerLerpValue, upperLerpValue, Main.rand.NextFloat());
            for (int i = 0; i < 50; i++)
            {
                projSpawnOffset = realPlayerPos + f.ToRotationVector2() * MathHelper.Lerp(lowerLerpValue, upperLerpValue, Main.rand.NextFloat());
                if (Collision.CanHit(realPlayerPos, 0, 0, projSpawnOffset + (projSpawnOffset - realPlayerPos).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                {
                    break;
                }
                f = Main.rand.NextFloat() * MathHelper.TwoPi;
            }
            Vector2 mouseWorld = Main.MouseWorld;
            Vector2 projSpawnPos = mouseWorld - projSpawnOffset;
            Vector2 projVelocity = new Vector2(mouseXDist, mouseYDist).SafeNormalize(Vector2.UnitY) * projSpeed;
            projSpawnPos = projSpawnPos.SafeNormalize(projVelocity) * projSpeed;
            projSpawnPos = Vector2.Lerp(projSpawnPos, projVelocity, 0.25f);
            Projectile.NewProjectile(source, projSpawnOffset, projSpawnPos, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SkyFracture, 1).
                AddRecipeGroup("MythrilBar", 14).
                AddIngredient(ItemID.SoulofMight, 7).
                AddIngredient(ItemID.SoulofLight, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}