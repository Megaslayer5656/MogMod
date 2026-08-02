using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    // TODO: rework
    public class BladeOfSelves : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        int numHits = 0;
        public override void SetDefaults()
        {
            Item.width = Item.height = 50;

            Item.damage = 94;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 60;
            Item.useTurn = true;
            Item.autoReuse = true;

            Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<EchoSabreHoldout>();
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            numHits++;
            if (numHits == 2)
            {
                MogModUtils.ProjectileBarrage(target.GetSource_FromAI(), target.Center, target.Center, Main.rand.NextBool(), 150f, 150f, -150f, 150f, 10f, ModContent.ProjectileType<SelvesProj1>(), Convert.ToInt32(Item.damage * 0.95), 0f, player.whoAmI, false, 0f);
                numHits = 0;
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<EchoSabre>(1).
                AddIngredient(ItemID.HallowedBar, 12).
                AddIngredient<UltimateOrb>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}