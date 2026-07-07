using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class BriarsOfPunishment : SorcerySpell
    {
        public override int ManaCost => 8;
        public override int AttackSpeed => 40;
        public override int PlayerHurtDamage => 4;
        public override string PlayerDeathReason => "was violently pricked by thorns.";
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 35;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<ShardSpiralProj>();
            Item.shootSpeed = 3f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodenSpike, 25).
                AddIngredient(ItemID.ChlorophyteBar, 8).
                AddIngredient<Scroll>(1).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}