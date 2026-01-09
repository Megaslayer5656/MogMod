using MogMod.Projectiles.RangedProjectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Ranged
{
    public class BloodGrenade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            //Item.velocity = 5.5f;
            Item.width = 14;
            Item.height = 20;
            Item.damage = 90;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.knockBack = 8f;
            Item.maxStack = Item.CommonMaxStack;
            Item.shootSpeed = 8f;
            Item.UseSound = SoundID.Item1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 0, 2, 50);
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ModContent.ProjectileType<BloodGrenadeProjectile>();
        }

        public override bool? UseItem(Player player)
        {
            // (death message, damage, hitDirection, pvp, quiet, cooldownCounter, dodgeable, armorPenetration, scalingArmorPenetration, knockback)
            player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " threw a blood grenade."), 5, -player.direction, false, false, -1, false, 0, 0, 0);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50).
                AddIngredient(ItemID.Grenade, 50).
                AddRecipeGroup("CrimtaneBar", 1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}