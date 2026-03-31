using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using static MogMod.Common.Systems.MogModNetcode;
using Terraria.DataStructures;

namespace MogMod.Items.Weapons.Melee
{
    public class ChaosBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random random = new Random();
        public int randUltraCrit;
        public int randNumProjectiles;
        public bool ultraCrit = false;
        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 30;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.scale = 1.5f;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            Item.crit = random.Next(1, 40);
            Item.damage = random.Next(25, 50); //This is hard to balance, more testing required
            randUltraCrit = random.Next(1, 13); //1 in 12 chance
            if (ultraCrit)
            {
                Rectangle r = new Rectangle((int)target.position.X, (int)target.position.Y - 50, target.width, target.height);
                Color textColor = new Color(255, 0, 0);
                CombatText.NewText(r, textColor, "Ultra Crit!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.UltraCritTextSync);
                    packet.Write(player.whoAmI);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
                randNumProjectiles = random.Next(1, 4);
                for (int i = 0; i < randNumProjectiles; i++)
                {
                    // proj barrage does (source, Vector2 originVec, Vector2 targetPos, T/F fromRight, xOffsetMin, xOffsetMax, yOffsetMin, yOffsetMax, projSpeed, projType, damage, knockback, owner, T/F clamped, innacuracy)
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, true, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<ChaosBladeProj>(), random.Next(40, 65), 0f, player.whoAmI, false, 0f);
                }
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
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 15).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Flesh"}", 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
