using System;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.Audio;
using MogMod.Utilities;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Items.Other;

namespace MogMod.Items.Weapons.Melee
{
    public class ChaosBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random random = new Random();
        public int randUltraCrit;
        public int randNumProjectiles;
        public bool ultraCrit = false;
        public static readonly SoundStyle UltraCrit = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/UltraCrit") //Shot sound effect
        {
            Volume = 1.1f,
            PitchVariance = .2f
        };
        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.useTurn = false;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.scale = 1.5f;
            Item.shootSpeed = 10f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            Item.crit = random.Next(1, 40);
            Item.damage = random.Next(15, 45);
            randUltraCrit = random.Next(1, 25);
            if (ultraCrit)
            {
                randNumProjectiles = random.Next(1, 4);
                for (int i = 0; i < randNumProjectiles; i++)
                {
                    // proj barrage does (source, Vector2 originVec, Vector2 targetPos, T/F fromRight, xOffsetMin, xOffsetMax, yOffsetMin, yOffsetMax, projSpeed, projType, damage, knockback, owner, T/F clamped, innacuracy)
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ChaosBladeProj>(), random.Next(40, 65), 0f, player.whoAmI, false, 0f);
                }
                SoundEngine.PlaySound(UltraCrit, player.Center);
                int heal = random.Next(1, 5);
                // for SOME REASON player has a default of 70 lifesteal
                heal *= Convert.ToInt32(player.lifeSteal * 0.08);
                player.statLife += heal;
                player.HealEffect(heal);
                // so we dont go over max life
                if (player.statLife > player.statLifeMax2)
                    player.statLife = player.statLifeMax2;
                ultraCrit = false;
            }
            if (randUltraCrit == 10)
            {
                Item.damage = random.Next(80, 130);
                Item.crit = 100;
                ultraCrit = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 20).
                AddRecipeGroup("CrimtaneBar", 15).
                AddRecipeGroup("TissueSample", 10).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
