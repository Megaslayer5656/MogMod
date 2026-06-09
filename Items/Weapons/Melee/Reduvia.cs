using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Magic;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    //Very important note: All of the blood stuff is set up in MogGlobalItem.cs, MogModGlobalProjectileBleed.cs, and MogModGlobalNPC.cs.
    public class Reduvia : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        int shotCounter = 0;
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 44;
            Item.damage = 45;
            Item.scale = 1.5f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3.5f;
            Item.rare = ItemRarityID.LightRed;
            Item.value = MogGlobalItem.RarityLightRedBuyPrice;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BloodMagicProjectile>();
            Item.shootSpeed = 10f;
        }
        public override bool CanShoot(Player player) => shotCounter == 2;
        public override bool? UseItem(Player player)
        {
            shotCounter++;
            if (shotCounter > 2)
                shotCounter = 0;
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
              AddIngredient(ItemID.Sickle).
              AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 15). // von squad;
              AddIngredient(ItemID.Bone, 10).
              AddIngredient<SpiritShard>(5).
              AddTile(TileID.Anvils).
              Register();
        }
    }
}