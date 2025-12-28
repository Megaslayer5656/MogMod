using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.Items.Accessories;
using MogMod.Items.Other;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Crystalys : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 55;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 13;
            Item.useTurn = false;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            //Item.value = 
            Item.rare = ItemRarityID.LightRed;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;

        //public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        //{
        //    Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/MajesticGuardGlow").Value);
        //}

        // lifesteal effect
        //public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        //{
        //    int heal = (int)(player.lifeSteal * .1f);
        //    player.statLife += heal;
        //    player.HealEffect(heal);
        //}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("AdamantiteBar", 28).
                AddIngredient<BladesOfAttack>(1).
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}