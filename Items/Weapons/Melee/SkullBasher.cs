using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using MogMod.Utilities;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;

namespace MogMod.Items.Weapons.Melee
{
    public class SkullBasher : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random random = new Random();
        public int randChance;
        public int randNumProjectiles;
        public bool skullBash = false;
        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.scale = 1f;
            Item.shootSpeed = 2f;
        }
        //public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 26;
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            Item.damage = 20;
            Item.crit = 6;
            randChance = random.Next(1, 4);
            if (skullBash)
            {
                target.AddBuff(BuffID.Dazed, 120);
                bool randomBool = random.Next(2) == 0;
                MogModUtils.ProjectileBarrage(source, target.Center, target.Center, randomBool, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<SkullBashProjectile>(), (Item.damage / 2), 0f, player.whoAmI, false, 0f);
                skullBash = false;
            }
            if (randChance == 2)
            {
                Item.damage += 30;
                Item.crit = 46;
                skullBash = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("IronBar", 20).
                AddRecipeGroup("CrimtaneBar", 18).
                AddIngredient(ItemID.Skull, 1).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
