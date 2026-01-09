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
    public class AbyssalBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random random = new Random();
        public int randChance;
        public int randNumProjectiles;
        public bool skullBash = false;
        public static readonly SoundStyle bashProc = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/SkullBash")
        {
            Volume = 1.3f,
            PitchVariance = .2f
        };
        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 92;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Cyan;
            Item.scale = 1.5f;
            Item.shootSpeed = 10f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            Item.damage = 68;
            Item.crit = 66;

            int heal = 1;
            heal *= Convert.ToInt32(player.lifeSteal * 0.08);
            player.statLife += heal;
            player.HealEffect(heal);
            if (player.statLife > player.statLifeMax2)
                player.statLife = player.statLifeMax2;

            randChance = random.Next(1, 4);
            if (skullBash)
            {
                target.AddBuff(BuffID.Dazed, 360);
                target.AddBuff(BuffID.Slow, 360);
                target.AddBuff(BuffID.BrokenArmor, 360);
                for (int i = 0; i < 4; i++)
                {
                    SoundEngine.PlaySound(bashProc, player.Center);
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<SkullBashProjectile>(), Convert.ToInt32(Item.damage / 3.2), 0f, player.whoAmI, false, 0f);
                }
                skullBash = false;
            }
            if (randChance == 2)
            {
                Item.damage += 182;
                Item.crit = 96;
                skullBash = true;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SkullBasher>(1).
                AddIngredient<Sange>(1).
                AddIngredient(ItemID.VampireKnives, 1).
                AddRecipeGroup("AdamantiteBar", 18).
                AddIngredient<CraftingRecipe>(1).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
