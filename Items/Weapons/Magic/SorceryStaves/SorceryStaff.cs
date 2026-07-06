using Microsoft.Xna.Framework;
using MogMod.Common.Systems;
using MogMod.Items.Ammo.SorcerySpells;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    // TODO: figure out how to make spell noita equal to nullscapes
    public abstract class SorceryStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Staves";
        public virtual float ManaCostMult => 1f;
        public virtual float AttackSpeedMult => 1f;
        public SorcerySpell Spell = ModContent.GetInstance<EmptySpell>();
        public override void UpdateInventory(Player player)
        {
            Item ammoItem = player.ChooseAmmo(Item);

            if (ammoItem.ModItem is SorcerySpell spell)
            {
                Spell = spell;
            }
            else
            {
                Spell = ModContent.GetInstance<EmptySpell>();
            }
        }
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 1;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 0f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // sorcery spell ammo types;
            Item.noMelee = true;
        }
        public override bool CanUseItem(Player player)
        {
            Item.useTime = Item.useAnimation = Spell.AttackSpeed;
            Item.UseSound = Spell.UseSound;
            Item.noUseGraphic = Spell.SwordStyle;
            return base.CanUseItem(player);
        }
        // change stats depending on what spell was casted
        public override float UseSpeedMultiplier(Player player)
        {
            if (Spell != null)
                return AttackSpeedMult;
            return 1f;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            if (Spell != null)
                mult = Spell.ManaCost * ManaCostMult;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Spell != null)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
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