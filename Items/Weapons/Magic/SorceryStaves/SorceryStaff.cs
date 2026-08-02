using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Items.Ammo.SorcerySpells;
using MogMod.Items.Ammo.SorcerySpells.Glintstone;
using MogMod.Items.Global;
using MogMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Weapons.Magic.SorceryStaves
{
    /// <summary>
    /// <see cref="SorceryStaff"/> is a custom <see cref="ModItem"/> primarily used for casting <see cref="SorcerySpell"/>, a non-consumable ammo type.
    /// </summary>
    /// <remarks>
    /// Items with this class automatically set a custom damage class: <see cref="SorceryDamageClass"/> 
    /// <br/>Have a custom tooltip: "A catalyst used to cast sorceries".
    /// <br/>And a custom localization category: "Items.Weapons.Staves".
    /// </remarks>
    public abstract class SorceryStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Staves";
        public int Owner { get; set; }
        /// <summary> The mana multiplier on any spell casted with this item.
        /// <br/>This value multiplies the <see cref="SorcerySpell.ManaCost"/> in the <see cref="ModifyManaCost"/> method. </summary>
        /// <remarks> Defaults to 1f. </remarks>
        public virtual float ManaCostMult => 1f;
        /// <summary> The attack speed multiplier on any spell casted with this item.
        /// <br/>This value is returned at the end of the <see cref="UseSpeedMultiplier"/> method. </summary>
        /// <remarks> Defaults to 1f. </remarks>
        public virtual float AttackSpeedMult => 1f;
        /// <summary> The velocity multiplier on any spell casted with this item.
        /// <br/>This value multiplies the velocity parameter in the <see cref="Shoot"/> method. </summary>
        /// <remarks> Defaults to 1f. </remarks>
        public virtual float VelocityMult => 1f;
        /// <summary> The knockback multiplier on any spell casted with this item.
        /// <br/>This value multiplies the knockback parameter in the <see cref="Shoot"/> method. </summary>
        /// <remarks> Defaults to 1f. </remarks>
        public virtual float KnockbackMult => 1f;
        /// <summary> How much damage this item does to the player when a sorcery is casted. </summary>
        /// <remarks> Defaults to 0. </remarks>
        public virtual int StaffSelfHurtDamage => 0;
        /// <summary> Use this to set a custom death message for if this item kills the player. </summary>
        /// <remarks> Be sure to modify <see cref="PlayerDeathReason.ByCustomReason(Terraria.Localization.NetworkText)"/>. </remarks>
        public virtual PlayerDeathReason StaffDeathReason => PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.SorceryStaff" + Main.rand.Next(1, 2 + 1)).ToNetworkText(Main.player[Owner].name));
        /// <summary> The specific spell instance set for each sorcery spell. (i think) </summary>
        public SorcerySpell Spell = ModContent.GetInstance<EmptySpell>();
        public override void UpdateInventory(Player player)
        {
            Item ammoItem = player.ChooseAmmo(Item);

            if (player.HasAmmo(Item))
            {
                if (ammoItem.ModItem is SorcerySpell spell)
                    Spell = spell;
                else
                    Spell = ModContent.GetInstance<EmptySpell>();
            }
        }
        public override void SetStaticDefaults()
        {
            Spell?.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.DamageType = SorceryDamageClass.Instance;
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 1;
            Item.knockBack = 1f;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 0f;
            Item.useAmmo = ModContent.ItemType<GlintstonePebble>(); // sorcery spell ammo types;
            Item.noMelee = true;
        }
        public override bool? UseItem(Player player)
        {
            Spell?.UseItem(player);
            return base.UseItem(player);
        }
        public override bool CanUseItem(Player player)
        {
            Item.useTime = Spell.AttackSpeed / Spell.NumberOfAttacks;
            Item.useAnimation = Spell.AttackSpeed;
            Item.reuseDelay = Spell.AttackDelay;
            Item.UseSound = Spell.UseSound;
            Item.noUseGraphic = Spell.SwordStyle;
            Item.channel = Spell.Channeled;
            return base.CanUseItem(player);
        }
        public override bool CanShoot(Player player)
        {
            if (Spell.OnlyOneActive)
                return player.ownedProjectileCounts[Spell.Item.shoot] <= 0;
            return base.CanShoot(player);
        }
        // change stats depending on what spell was casted
        public override float UseSpeedMultiplier(Player player)
        {
            Item.TryGetGlobalItem<SorceryStaffGlobalItem>(out var sorceryStaff);
            if (Spell != null)
                return AttackSpeedMult * sorceryStaff.AttackSpeedPrefixBonus;
            return 1f;
        }
        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            Item.TryGetGlobalItem<SorceryStaffGlobalItem>(out var sorceryStaff);
            if (Spell != null)
                mult = Spell.ManaCost * (ManaCostMult * sorceryStaff.ManaCostPrefixBonus);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item.TryGetGlobalItem<SorceryStaffGlobalItem>(out var sorceryStaff);
            Spell?.Shoot(player, source, position, velocity * (VelocityMult * sorceryStaff.VelocityPrefixBonus), type, damage, knockback * (KnockbackMult * sorceryStaff.KnockbackPrefixBonus));
            if (Spell.SpellSelfHurtDamage > 0)
            {
                player.Hurt(Spell.SpellDeathReason, Spell.SpellSelfHurtDamage, -player.direction, false, false, -1, false, 9999, 0, 0);
                player.immune = false;
                player.immuneTime = 0;
            }
            if (StaffSelfHurtDamage > 0 || sorceryStaff.SelfHurtPrefixBonus > 0)
            {
                player.Hurt(StaffDeathReason, StaffSelfHurtDamage + sorceryStaff.SelfHurtPrefixBonus, -player.direction, false, false, -1, false, 9999, 0, 0);
                player.immune = false;
                player.immuneTime = 0;
            }
            return false;
        }
        public override bool WeaponPrefix() => true;
        public override bool MagicPrefix() => false;
        // remove unnecessary tooltips && add a custom tooltip line
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var player = Main.LocalPlayer;
            if (player != null)
            {
                var cataLine = new TooltipLine(Mod, "Catalyst", "A catalyst used to cast sorceries");
                // removes the items base mana, use speed, and knockback tooltips
                tooltips.RemoveRange(3, 3);
                tooltips.Insert(3, cataLine);
            }
        }
    }
}