using Microsoft.Xna.Framework;
using MogMod.Items.Global;
using MogMod.Items.Placeable;
using MogMod.Projectiles.MeleeProjectiles;
using MogMod.Utilities;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static MogMod.Common.Systems.MogModNetcode;

namespace MogMod.Items.Weapons.Melee
{
    // might resprite
    public class AbyssalBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        Random rand = new Random();
        public bool bashProc = false;
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
            Item.value = MogGlobalItem.RarityCyanBuyPrice;
            Item.scale = 1.5f;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileID.PurificationPowder; //This (and the shoot method) just make the weapon be able to face the direction of your mouse when you swing
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_OnHit(target);
            bashProc = rand.Next(4) == 0;
            if (bashProc)
            {
                for (int i = 0; i < 4; i++)
                {
                    bool randomBool = rand.Next(2) == 0;
                    MogModUtils.ProjectileBarrage(source, target.Center, target.Center, randomBool, 50f, 50f, -50f, 100f, 0.25f, ModContent.ProjectileType<AbyssalBladeProj>(), Convert.ToInt32(Item.damage / 2), 0f, player.whoAmI, false, 0f);
                }
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
                AddIngredient<SkullBasher>(1).
                AddIngredient<Sange>(1).
                AddIngredient(ItemID.VampireKnives, 1).
                AddRecipeGroup("AdamantiteBar", 15).
                AddIngredient<GriefBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
