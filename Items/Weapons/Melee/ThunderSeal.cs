using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using MogMod.Utilities;
using Mono.Cecil;
using MogMod.Items.Other;
using MogMod.Utilities;
using MogMod.Buffs.Debuffs;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Projectiles.RangedProjectiles;

namespace MogMod.Items.Weapons.Melee
{
    public class ThunderSeal : ModItem
    {
        SoundStyle shockStateMeleeProc = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/ShockStateMeleeProc")
        {
            Volume = .67f,
            PitchVariance = .02f,
        };
        public override void SetDefaults()
        {
            Item.width = 99;
            Item.height = 100;
            Item.damage = 60;
            Item.scale = .65f;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7f;
            Item.value = Item.buyPrice(0, 65, 0, 0);
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item1;
            Item.shootSpeed = 5f;
            Item.autoReuse = true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = 0;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = false;
            if (player.altFunctionUse == 2)
            {
                //Somehow make it not melee
                Item.useTime = 60;
                Item.useAnimation = 60;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.shoot = ModContent.ProjectileType<StunEdge>();
                Item.UseSound = new SoundStyle($"{nameof(MogMod)}/Sounds/SE/StunEdge")
                {
                    Volume = .67f,
                    PitchVariance = .02f,
                };
                Item.noMelee = true;
            } else
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.shoot = 0;
                Item.UseSound = SoundID.Item1;
                Item.noMelee = false;
            }
            return true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.HasBuff<ShockState>())
            {
                //TODO: Eventually make it a lightning strike instead of a dust particle attack
                var source = target.GetSource_FromAI();
                for (int x = 0; x < 4; x++)
                {
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ThunderSealProj>(), 75, 0f, player.whoAmI, false, 0f);
                }
                target.DelBuff(target.FindBuffIndex(ModContent.BuffType<ShockState>()));
                SoundEngine.PlaySound(shockStateMeleeProc, target.Center);
            }
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            damage = 20;
        }

        public override void AddRecipes()
        {
            //TODO: Make this recipe more interesting but not too grindy
            CreateRecipe().
                AddIngredient(ItemID.SoulofLight, 10).
                AddIngredient(ItemID.SoulofFlight, 5).
                AddRecipeGroup("CobaltBar", 15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
