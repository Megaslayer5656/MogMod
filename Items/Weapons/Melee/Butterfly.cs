using MogMod.Buffs.Cooldowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MogMod.Buffs.PotionBuffs;
using Mono.CompilerServices.SymbolWriter;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Utilities;
using Terraria.Localization;

namespace MogMod.Items.Weapons.Melee
{
    public class Butterfly : ModItem
    {
        Random rand = new Random();
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 205;
            Item.damage = 90;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useTurn = false;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Cyan;
            Item.scale = 1f;
        }

        public override bool AltFunctionUse(Player player)
        {
            if (!player.HasBuff<ButterflyCooldown>())
            {
                player.AddBuff(ModContent.BuffType<ButterflyBuff>(), 60);
                player.AddBuff(ModContent.BuffType<ButterflyCooldown>(), 600);
                return true;
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = target.GetSource_FromAI();
            if (Main.rand.NextBool(5))
            {
                for (int i = 0; i <= 3; i++)
                {
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, Main.rand.NextBool(2), -400f, 400f, -300f, 300f, 5.75f, ModContent.ProjectileType<ButterflyProjectile>(), Convert.ToInt32(Item.damage / 5f), 0f, player.whoAmI, false, 0f);
                }
            }
        }

        public override void AddRecipes()
        {
            //TODO: Make it so this recipe can take any butterfly (recipe group)
            CreateRecipe().
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Butterfly"}", 1).
                AddIngredient(ItemID.ChlorophyteBar, 15).
                AddIngredient(ItemID.Ectoplasm, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}