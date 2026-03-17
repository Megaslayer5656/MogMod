using Microsoft.Xna.Framework;
using MogMod.Items.Other;
using MogMod.Projectiles.MeleeProjectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Weapons.Melee
{
    public class SkullBasher : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random rand = new Random();
        public bool bashProc = false;
        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 20;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 33;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = false;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.LightRed;
            Item.scale = 1f;
            Item.shootSpeed = 2f;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return false;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            bashProc = rand.Next(9) == 0;
            if (bashProc)
            {
                int bash = Projectile.NewProjectile(source, target.Center, new Vector2(10f, 10f), ModContent.ProjectileType<SkullBashProjectile>(), Item.damage * 5, 0f, player.whoAmI);
                Rectangle r = new Rectangle((int)target.position.X - 10, (int)target.position.Y - 50, target.width, target.height);
                Color textColor = new Color(255, 0, 75);
                CombatText.NewText(r, textColor, "Bash!", true);
                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)MogModMessageType.BashProcTextSync);
                    packet.Write(target.lastInteraction);
                    packet.WriteVector2(r.Center.ToVector2());
                    packet.Send();
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddRecipeGroup("IronBar", 20).
                AddRecipeGroup($"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}", 18).
                AddIngredient(ItemID.Skull, 1).
                AddIngredient<VitalityBooster>(1).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
