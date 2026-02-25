using MogMod.Common.MogModPlayer;
using MogMod.Items.Other;
using MogMod.Items.Weapons.Magic;
using MogMod.Items.Weapons.Melee;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class KayaAndSange : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Purple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // increase size of melee weapons
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingSange = true;

            player.statLifeMax2 += 30;
            player.lifeRegen += 4;
            player.GetDamage(DamageClass.Generic) += .16f;
            player.GetDamage(DamageClass.Magic) += .12f;
            player.manaRegen += (int)Math.Round(player.manaRegen * .5f);
            player.statManaMax2 += 70;
            player.lifeSteal *= 1.25f;

            // not in ankh shield
            player.buffImmune[BuffID.Venom] = true;
            player.buffImmune[BuffID.Webbed] = true;
            player.buffImmune[BuffID.Blackout] = true;
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.OnFire3] = true;
            player.buffImmune[BuffID.Chilled] = true;
            player.buffImmune[BuffID.Frozen] = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[BuffID.Frostburn2] = true;
            player.buffImmune[BuffID.CursedInferno] = true;
            player.buffImmune[BuffID.ShadowFlame] = true;
            player.buffImmune[BuffID.Daybreak] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Kaya>(1).
                AddIngredient<Sange>(1).
                AddIngredient<GriefBar>(7).
                AddIngredient(ItemID.Ectoplasm, 3).
                AddIngredient<PointBooster>(1).
                AddIngredient<VitalityBooster>(1).
                AddIngredient<ManaEssence>(1).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
