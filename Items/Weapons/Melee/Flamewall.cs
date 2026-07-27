using Microsoft.Xna.Framework;
using MogMod.Buffs.Debuffs;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Melee;
using MogMod.Rarities;
using MogMod.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Melee
{
    public class Flamewall : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = Item.height = 134;
            Item.damage = 220;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
            Item.scale = 1.2f;
            Item.shootSpeed = 14f;
            Item.shoot = ModContent.ProjectileType<FlamewallProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item70, player.Center);
            float speed = velocity.Length();
            int meteorProj = ModContent.ProjectileType<FlameMeteorProj>();
            for (int i = 0; i < 6; ++i)
            {
                float randSpeed = speed * Main.rand.NextFloat(0.7f, 1.4f);
                MogModUtils.ProjectileRain(source, Main.MouseWorld, 300f, 150f, 850f, 1100f, randSpeed, meteorProj, damage, knockback, player.whoAmI);
            }
            float adjustedItemScale = player.GetAdjustedItemScale(Item); // Get the melee scale of the player and item.
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), type, damage, knockback, player.whoAmI, player.direction * player.gravDir, player.itemAnimationMax, adjustedItemScale);
            NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI); // Sync the changes in multiplayer.

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 46;
        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers) => modifiers.CritDamage *= 1.5f;
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 300);
            OnHitEffects(player, target.Center);
        }
        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<InfernoDebuff>(), 300);
            OnHitEffects(player, target.Center);
        }
        private void OnHitEffects(Player player, Vector2 targetPos)
        {
            var source = player.GetSource_ItemUse(Item);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<FlameTornadoProj>()] == 0)
                Projectile.NewProjectile(source, targetPos, Vector2.Zero, ModContent.ProjectileType<FlamePortal>(), Item.damage, Item.knockBack, player.whoAmI);
        }
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            Lighting.AddLight(new Vector2(hitbox.X, hitbox.Y), 2f, 1f, 1f);

            if (Main.rand.NextBool(2))
            {
                int d = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Flare);
            }
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            List<Color> colorList = new List<Color>()
            {
                new Color(214, 92, 92),
                new Color(209, 146, 59),
                new Color(217, 195, 74),
            };
            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip4");
            if (line != null)
                line.OverrideColor = Color.Lerp(tooltipColor, Color.White, 0.5f);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Daedalus>(1).
                AddIngredient(ItemID.TheAxe, 1).
                AddIngredient<Flamebrand>(1).
                AddIngredient<VoniumBar>(5).
                AddIngredient<SoulOfMogMod>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}