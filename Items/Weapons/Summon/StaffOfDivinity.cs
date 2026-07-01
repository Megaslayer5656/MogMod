using System.Linq;
using Microsoft.Xna.Framework;
using MogMod.Buffs.Summons;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.Summon;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Summon
{
    public class StaffOfDivinity : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = Item.height = 64;

            Item.mana = 10;
            Item.damage = 120;
            Item.DamageType = DamageClass.Summon;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 6f;

            Item.UseSound = SoundID.Item44;
            Item.buffType = ModContent.BuffType<DivinitasSummonBuff>();
            Item.shoot = ModContent.ProjectileType<DivinitasSummon>();

            Item.rare = ItemRarityID.Purple;
            Item.value = MogGlobalItem.RarityPurpleBuyPrice;
        }
        public override bool CanUseItem(Player player)
        {
            float minionSlotsAvailable = player.maxMinions;
            foreach (var item in Main.ActiveProjectiles)
            {
                if (item.owner == player.whoAmI)
                    minionSlotsAvailable -= item.minionSlots;
            }
            return minionSlotsAvailable >= 1;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] > 0)
            {
                var p = Main.projectile.First(x => x.active && x.type == type && x.owner == player.whoAmI);
                p.ai[0]++;
                p.netUpdate = true;
                return false;
            }
            player.AddBuff(Item.buffType, 2);
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LightDisc).
                AddIngredient(ItemID.LunarBar, 12).
                AddIngredient<SoulFragment>(3).
                AddIngredient<SoulOfMogMod>(1).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
