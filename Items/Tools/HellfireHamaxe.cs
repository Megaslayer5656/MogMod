using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Tools
{
    public class HellfireHamaxe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 54;

            Item.damage = 40;
            Item.knockBack = 6.5f;
            Item.DamageType = DamageClass.Melee;

            Item.useTime = 6;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Lime;
            Item.value = MogGlobalItem.RarityLimeBuyPrice;

            Item.useTurn = true;
            Item.autoReuse = true;
                
            Item.axe = 24; // in game value is 5x this
            Item.hammer = 90;
            Item.attackSpeedOnlyAffectsWeaponAnimation = true; // melee speed affects how fast the tool swings for damage purposes, but not how fast it can dig
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool(3) ? DustID.Lava : 174);
        }
        public override void AddRecipes() // hallowed tier
        {
            CreateRecipe().
               AddIngredient<HellfireBar>(10).
               AddTile(TileID.MythrilAnvil).
               Register();
        }
    }
}
