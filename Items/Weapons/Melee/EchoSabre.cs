using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.RangedProjectiles;
using MogMod.Utilities;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class EchoSabre : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random random = new Random();
        public int randChance;
        public bool echoSabre = false;
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.scale = 1.15f;
            Item.shootSpeed = 10f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            // yashaProj for now but make it its own proj later
            MogModUtils.ProjectileBarrage(source, target.Center, target.Center, false, 70f, 70f, -70f, 120f, 0.30f, ModContent.ProjectileType<YashaProj>(), Convert.ToInt32(Item.damage * 0.95), 0f, player.whoAmI, false, 0f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("CobaltBar", 22).
                AddIngredient(ItemID.SoulofLight, 6).
                AddIngredient(ItemID.SoulofNight, 6).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
