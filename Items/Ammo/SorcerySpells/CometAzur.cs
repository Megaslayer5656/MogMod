using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.MagicProjectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class CometAzur : SorcerySpell
    {
        public override int ManaCost => 4;
        public override int AttackSpeed => 40;
        public override bool Channeled => true;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 35;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 3f;
            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
            Item.shoot = ModContent.ProjectileType<PhylacteryBeam>();
            Item.shootSpeed = 3f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient<SoulOfMogMod>().
                AddIngredient<Scroll>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}