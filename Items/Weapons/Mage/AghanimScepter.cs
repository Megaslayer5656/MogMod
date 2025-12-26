using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using MogMod.Items.Other;
using MogMod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace MogMod.Items.Weapons.Mage
{
    public class AghanimScepter : ModItem
    {
        // CHANGE SPRITE TO WAND
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
        }
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.damage = 42;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 40;
            Item.useTime = 2;
            Item.useAnimation = 15;
            Item.reuseDelay = Item.useAnimation + 2;
            Item.useLimitPerAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 9f;
            Item.value = Item.buyPrice(0, 7, 30, 50);
            Item.rare = ItemRarityID.Purple;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AghanimProjectile>();
            Item.shootSpeed = 22f;
        }

        // here instead of SetDefaults since terraria doesnt like it being in there for some reason
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 36;

        // copy pasted from kaya in mogmod
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
                AddIngredient<Kaya>(1).
                AddIngredient<AghanimShard>(1).
                AddIngredient(ItemID.PossessedHatchet, 1).
                AddIngredient(ItemID.CrystalShard, 20).
                AddIngredient(ItemID.UnicornHorn, 5).
                AddIngredient<PointBooster>(1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}