using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    // needs a bigger sprite
    // right click will give kraken shell (10% damage reduction buff that lasts 3 seconds)
    public class OversizedAnchor : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public int attackType = 0; // keeps track of which attack it is
        public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time
        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;

            Item.damage = 78;
            Item.knockBack = 10f;
            Item.DamageType = DamageClass.Melee;
            
            Item.useTime = Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            
            Item.shoot = ModContent.ProjectileType<AnchorHoldout>();
            Item.shootSpeed = 1f;
            

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;

            Item.noMelee = true;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int anchor = ModContent.ProjectileType<AnchorHoldout>();
            if (player.altFunctionUse == 2)
            {
                attackType = 2;
                comboExpireTimer = 119;
                Projectile.NewProjectile(source, position, velocity, anchor, damage, knockback, Main.myPlayer, attackType);
            }
            if (attackType <= 1)
            {
                // do this so different rockets don't mess with the projectile spawned
                // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
                Projectile.NewProjectile(source, position, velocity, anchor, damage, knockback, Main.myPlayer, attackType);
                attackType = (attackType + 1) % 2; // Increment attackType to make sure next swing is different
                comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
            }
            return false; // return false to prevent original projectile from being shot
        }
        public override void UpdateInventory(Player player)
        {
            if (comboExpireTimer++ >= 120) // after 120 ticks (== 2 seconds) in inventory, reset the attack pattern
                attackType = 0;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool MeleePrefix() => true;

        // added an anchor to the recipe but made anchors craftable
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Anchor, 1).
                AddIngredient(ItemID.SharkFin, 5).
                AddIngredient<UltimateOrb>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}