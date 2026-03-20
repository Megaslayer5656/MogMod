using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Gunlance : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public int attackType = 0; // keeps track of which attack it is
        public int comboExpireTimer = 0; // we want the attack pattern to reset if the weapon is not used for certain period of time
        public static bool Blast = false;
        public static readonly SoundStyle SwingSound = SoundID.DD2_PhantomPhoenixShot;
        public static readonly SoundStyle SwingSound2 = SoundID.DD2_SkyDragonsFurySwing;
        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 90;
            Item.damage = 128;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.knockBack = 10f;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<GunlanceHoldout>();
            Item.shootSpeed = 1f;
            //Item.useAmmo = AmmoID.Rocket;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (attackType <= 1)
            {
                // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer, attackType);
                attackType = (attackType + 1) % 3; // Increment attackType to make sure next swing is different
                comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
                return false; // return false to prevent original projectile from being shot
            }
            else
            {
                attackType = (attackType + 1) % 3; // Increment attackType to make sure next swing is different
                comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
                int rapierBeam = ModContent.ProjectileType<GunlanceSpear>();
                Projectile.NewProjectile(source, position, velocity * 6f, rapierBeam, damage, knockback, player.whoAmI, 0, 0);
                return false;
            }
        }
        // load gunshells
        public override bool CanUseItem(Player player)
        {
            var mogPlayerUI = player.GetModPlayer<MogPlayerUI>();
            if (player.altFunctionUse == 2 && mogPlayerUI.gunlanceCurrent < mogPlayerUI.exampleResourceMax)
            {
                SoundEngine.PlaySound(SoundID.Item23);
                mogPlayerUI.gunlanceCurrent = mogPlayerUI.exampleResourceMax;
                Blast = true;
                // TODO: make this consume 3 ammo on right click only
                return false;
            }
            else
                return true;
        }
        //public override bool CanConsumeAmmo(Item ammo, Player player)
        //{
        //    if (player.altFunctionUse == 2)
        //        return true;
        //    else
        //        return false;
        //}
        public override bool AltFunctionUse(Player player) => true;
        public override void UpdateInventory(Player player)
        {
            if (comboExpireTimer++ >= 120) // after 120 ticks (== 2 seconds) in inventory, reset the attack pattern
            {
                attackType = 0;
            }
        }
        public override bool MeleePrefix() => true;
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChargedBlasterCannon, 1).
                AddIngredient(ItemID.FireFeather, 1).
                AddIngredient(ItemID.IceFeather, 1).
                AddIngredient(ItemID.BrokenHeroSword, 1).
                AddIngredient<ManaCore>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
