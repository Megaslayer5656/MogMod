using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Items.Placeable.Bars;
using MogMod.Projectiles.Melee;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Weapons.Melee
{
    // dabdadly (desperate) needs balancing
    public class ChaosArbiter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public static int numb;
        public static int strikeMin = 60;
        public static int strikeMax = 120;
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 9));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 72;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = MogGlobalItem.RarityYellowBuyPrice;
            Item.scale = 1.5f;
            Item.shootSpeed = 10f;
            Item.shoot = ModContent.ProjectileType<ChaosBoltProj>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.crit = Main.rand.Next(10, 40);
            Item.damage = Main.rand.Next(strikeMin, strikeMax);
            // clone shooting
            foreach (Projectile p in Main.ActiveProjectiles)
                if (p.type == ModContent.ProjectileType<ChaosArbiterClone>() && p.owner == player.whoAmI && Main.rand.Next(0, 3) == 0) // 1 in 3
                    p.ai[1] = 1f;
            if (Main.rand.Next(0, 3) == 0) // 1 in 3
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ChaosBoltProj>(), damage, knockback, Main.myPlayer, Main.rand.Next(0,6)); // 5 types of bolts
            return false;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);

            if (hit.Crit)
            {
                if (Main.rand.Next(0, 7) == 0) // 1 in 10 chance
                {
                    if (target.type != NPCID.TargetDummy)
                    {
                        int heal = Main.rand.Next(1, 5);
                        // for SOME REASON player has a default of 70 lifesteal
                        heal *= Convert.ToInt32(player.lifeSteal * 0.1);
                        player.statLife += heal;
                        player.HealEffect(heal);
                        // so we dont go over max life
                        if (player.statLife > player.statLifeMax2)
                            player.statLife = player.statLifeMax2;
                    }
                    // TODO: make phantom spawns take up empty slots instead of random
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ChaosArbiterClone>()] <= 3)
                    {
                        numb = Main.rand.Next(0, 4);
                        Projectile clone = Projectile.NewProjectileDirect(Item.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<ChaosArbiterClone>(), Item.damage, Item.knockBack, player.whoAmI, numb);
                        clone.OriginalCritChance = Item.crit;
                    }
                    if (Main.netMode == NetmodeID.Server)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)MogModMessageType.UltraCritTextSync);
                        packet.Write(target.lastInteraction);
                        packet.Write(target.whoAmI);
                        packet.Send();
                    }
                    else
                    {
                        target.MogMod().UltraCritFX(target);
                    }
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChaosBlade>().
                AddIngredient<GriefBar>(12).
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient<LizhardBloodVial>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}