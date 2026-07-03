using Microsoft.Xna.Framework;
using MogMod.Items.Ammo.SorcerySpells;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    // TODO: fix it not shooting anything
    public abstract class SorceryStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Staves";
        protected SorcerySpell Spell { get; }
        public virtual float ManaCostMult => 1f;
        public virtual float AttackSpeedMult => 1f;
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 1;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 0f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // sorcery spell ammo types;
            Item.noMelee = true;
        }
        // change stats depending on what spell was casted
        public override bool CanUseItem(Player player)
        {
            Item.noUseGraphic = !Spell.SwordStyle;
            Item.mana = (int)(Spell.ManaCost * ManaCostMult);
            Item.useTime = Item.useAnimation = (int)(Spell.AttackSpeed * AttackSpeedMult);
            Item.UseSound = Spell.UseSound;
            return base.CanUseItem(player);
        }
        // custom swing style for sword spells
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Spell.SwordStyle)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, Main.myPlayer);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        // remove unnecessary tooltips && add a custom tooltip line
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var player = Main.LocalPlayer;
            if (player != null)
            {
                var cataLine = new TooltipLine(Mod, "Catalyst", "A catalyst used to cast sorceries");
                string[] lLine = { "uses", "speed", "knockback" };
                tooltips.RemoveAll(line => lLine.Any(word => line.Text.ToLower().Contains(word.ToLower())));
                tooltips.Insert(3, cataLine);
            }
        }
    }
}