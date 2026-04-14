using Microsoft.Xna.Framework;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Placeable;
using MogMod.Projectiles.MeleeProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
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
        public static readonly int Bang = ModContent.ProjectileType<GunlanceBoom>();
        public override void SetDefaults()
        {
            Item.width = 94;
            Item.height = 90;

            Item.damage = 150;
            Item.knockBack = 10f;
            Item.DamageType = DamageClass.Melee;

            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.noMelee = true;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;

            Item.shoot = ModContent.ProjectileType<GunlanceHoldout>();
            Item.shootSpeed = 1f;

            Item.useAmmo = AmmoID.Rocket;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var mogPlayerUI = player.GetModPlayer<MogPlayerUI>();
            if (player.altFunctionUse == 2)
            {
                int reload = ModContent.ProjectileType<GunlanceReload>();
                Projectile.NewProjectile(source, position, velocity, reload, damage, knockback, Main.myPlayer, attackType);
                if (mogPlayerUI.gunlanceCurrent < mogPlayerUI.exampleResourceMax && player.HasItem(source.AmmoItemIdUsed))
                {
                    if (player.CountItem(source.AmmoItemIdUsed) >= (mogPlayerUI.exampleResourceMax - mogPlayerUI.gunlanceCurrent))
                    {
                        for (int n = mogPlayerUI.gunlanceCurrent; n < mogPlayerUI.exampleResourceMax; n++)
                        {
                            player.ConsumeItem(source.AmmoItemIdUsed);
                            mogPlayerUI.gunlanceCurrent++;
                        }
                        Blast = true;
                    }
                }
                attackType = 1;
                comboExpireTimer = 0;
                return false;
            }
            if (attackType <= 1)
            {
                // do this so different rockets don't mess with the projectile spawned
                int gunlance = ModContent.ProjectileType<GunlanceHoldout>();
                // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
                Projectile.NewProjectile(source, position, velocity, gunlance, damage, knockback, Main.myPlayer, attackType);
                attackType = (attackType + 1) % 3; // Increment attackType to make sure next swing is different
                comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
                return false; // return false to prevent original projectile from being shot
            }
            else
            {
                attackType = (attackType + 1) % 3; // Increment attackType to make sure next swing is different
                comboExpireTimer = 0; // Every time the weapon is used, we reset this so the combo does not expire
                // custom spear projectile since its easier
                int spear = ModContent.ProjectileType<GunlanceSpear>();
                Projectile.NewProjectile(source, position, velocity * 6f, spear, (int)(damage * 1.75f), knockback, player.whoAmI, 0, 0);
                return false;
            }
        }
        public override void UpdateInventory(Player player)
        {
            if (comboExpireTimer++ >= 120) // after 120 ticks (== 2 seconds) in inventory, reset the attack pattern
                attackType = 0;
        }
        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
        public override bool AltFunctionUse(Player player) => true;
        public override bool MeleePrefix() => true;

        // no idea when this can be obtained
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ExplosivePowder, 150).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Rocket"}", 80).
                AddIngredient<GriefBar>(14).
                AddIngredient<FuciumBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}