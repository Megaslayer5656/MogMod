using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class BladesOfStone : SorcerySpell
    {
        public override int ManaCost => 20;
        public override int AttackSpeed => 40;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 35;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
            Item.shoot = ModContent.ProjectileType<ShardSpiralProj>();
            Item.shootSpeed = 3f;
        }
        /* Not finished yet
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PixieDust, 25).
                AddRecipeGroup("MythrilBar", 15).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
        */
    }
}