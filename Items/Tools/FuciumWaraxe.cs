using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Tools
{
    public class FuciumWaraxe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 40;

            Item.damage = 32;
            Item.knockBack = 6;
            Item.DamageType = DamageClass.Melee;

            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;

            Item.useTurn = true;
            Item.autoReuse = true;

            Item.axe = 20; // in game value is 5x this
            Item.attackSpeedOnlyAffectsWeaponAnimation = true; // melee speed affects how fast the tool swings for damage purposes, but not how fast it can dig
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(10))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.BubbleBurst_Pink);
        }
        public override void AddRecipes() // hellstone tier
        {
            CreateRecipe().
               AddIngredient<FuciumBar>(10).
               AddTile(TileID.Anvils).
               Register();
        }
    }
}
