using Microsoft.Xna.Framework;
using MogMod.Common.Classes;
using MogMod.Items.Ammo.SorcerySpells.Glintstone;
using MogMod.Items.Weapons.Magic.SorceryStaves;
using MogMod.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static AssGen.Assets;

namespace MogMod.Items.Ammo.SorcerySpells
{
    /// <summary> <see cref="SorcerySpell"/> is a custom <see cref="ModItem"/> primarily used as ammo for <see cref="SorceryStaff"/>. </summary>
    /// <remarks> Items with this class automatically have: 
    /// <br/> A custom damage class: <see cref="SorceryDamageClass"/>
    /// <br/> A custom tooltip: "Sorcery".
    /// <br/> A custom localization category: "Items.Ammo.Spells". </remarks>
    public abstract class SorcerySpell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo.Spells";
        public int Owner { get; set; }
        /// <summary> The mana cost required to cast this spell. </summary>
        /// <remarks> Defaults to 1. </remarks>
        public virtual int ManaCost => 1;
        /// <summary> The time span of casting the spell.
        /// <br/>This value is used in the <see cref="SorceryStaff"/>'s <see cref="useTime"/> and <see cref="useAnimation"/>. </summary>
        /// <remarks> Defaults to 10. </remarks>
        public virtual int AttackSpeed => 10;
        /// <summary> How many times this spell should attack.
        /// <br/>This value divides the <see cref="SorceryStaff"/>'s <see cref="useTime"/> by it's value. </summary>
        /// <remarks> Defaults to 0. </remarks>
        public virtual int NumberOfAttacks => 1;
        /// <summary> The delay in casting this spell.
        /// <br/>This value is added to the <see cref="SorceryStaff"/>'s <see cref="reuseDelay"/>. </summary>
        /// <remarks> Defaults to 0. </remarks>
        public virtual int AttackDelay => 0;
        /// <summary> The amount of damage this spell does to the player when casted. </summary>
        /// <remarks> Defaults to 0. </remarks>
        public virtual int SpellSelfHurtDamage => 0;
        /// <summary> Use this to set a custom death message for if this spell kills the player. </summary>
        /// <remarks> Be sure to modify <see cref="PlayerDeathReason.ByCustomReason(Terraria.Localization.NetworkText)"/>. </remarks>
        public virtual PlayerDeathReason SpellDeathReason => PlayerDeathReason.ByCustomReason(MiscUtils.GetText("Status.Death.SorcerySpell" + Main.rand.Next(1, 2 + 1)).ToNetworkText(Main.player[Owner].name));
        /// <summary> The sound this spell makes when casted. </summary>
        /// <remarks> Defaults to <see cref="SoundID.Item8"/>. </remarks>
        public virtual SoundStyle UseSound => SoundID.Item8;
        // for spells that swing swords
        /// <summary> Whether or not this spell has a custom swing animation. </summary>
        /// <remarks> Defaults to <see langword="false"/>. </remarks>
        public virtual bool SwordStyle => false;
        /// <summary> Whether or not this spell has special effects when channeled. </summary>
        /// <remarks> Defaults to <see langword="false"/>. </remarks>
        public virtual bool Channeled => false;
        /// <summary> Boolean to cancel spell usage if there is one or more of the spell's projectile active. </summary>
        /// <remarks> Defaults to <see langword="false"/>. </remarks>
        public virtual bool OnlyOneActive => false;
        /// <summary> The Sorcery ID this spell belongs to. 
        /// <br/> Primarily used for adding a custom tooltip.
        /// <br/> Defaults to <see cref="SorceryID.None"/>. </summary>
        public int SorceryClass = SorceryID.None;
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = ManaCost;
            Item.DamageType = SorceryDamageClass.Instance;
            // so it can be used by sorcery staves;
            Item.ammo = ModContent.ItemType<GlintstonePebble>();
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit -= 4;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        // replaces the "Ammo" description with "Sorcery" since i dont think you can do it in localization;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            List<Color> legendaryColorList = [
                new(36, 101, 150),
                new(36, 40, 150),
                new(125, 36, 150),
            ];
            List<Color> thornColorList = [
                new(212, 66, 118),
                new(212, 66, 66),
            ];
            List<Color> magmaColorList = [
                new(212, 87, 108),
                new(212, 108, 87),
                new(212, 160, 87),
            ];
            List<Color> carianColorList = [
                new(71, 152, 209),
                new(71, 101, 209),
            ];
            List<Color> deathColorList = [
                new(142, 161, 103),
                new(103, 161, 111),
                new(103, 161, 143),
            ];
            List<Color> gravityColorList = [
                new(151, 129, 201),
                new(194, 129, 201),
            ];
            List<Color> glintstoneColorList = [
                new(171, 255, 238),
                new(171, 223, 255),
            ];
            List<Color> noColorList = [
                Color.White,
                Color.Black,
            ];
            List<Color> colorList = SorceryClass == SorceryID.Legendary ? legendaryColorList : 
                SorceryClass == SorceryID.Thorn ? thornColorList :
                SorceryClass == SorceryID.Magma ? magmaColorList :
                SorceryClass == SorceryID.Carian ? carianColorList :
                SorceryClass == SorceryID.Death ? deathColorList :
                SorceryClass == SorceryID.Gravity ? gravityColorList :
                SorceryClass == SorceryID.Glintstone ? glintstoneColorList : noColorList;
            int colorIndex = (int)(Main.GlobalTimeWrappedHourly / 2 % colorList.Count);
            Color currentColor = colorList[colorIndex];
            Color nextColor = colorList[(colorIndex + 1) % colorList.Count];
            Color tooltipColor = Color.Lerp(currentColor, nextColor, Main.GlobalTimeWrappedHourly % 2f > 1f ? 1f : Main.GlobalTimeWrappedHourly % 1f);
            var sorceryLine = tooltips.FirstOrDefault(x => x.Name == "Ammo" && x.Mod == "Terraria");
            if (sorceryLine != null)
            {
                sorceryLine.Text = MiscUtils.GetText("SorceryClasses.SorceryID" + SorceryClass).ToString();
                sorceryLine.OverrideColor = Color.Lerp(tooltipColor, tooltipColor * 0.75f, 0.5f);
            }
        }
    }
    /// <summary> What class of sorcery this spell is. Each class has a unique tooltip. </summary>
    public static partial class SorceryID
    {
        public static int None = 0;
        public static int Glintstone = 1;
        public static int Gravity = 2;
        public static int Death = 3;
        public static int Carian = 4;
        public static int Magma = 5;
        public static int Thorn = 6;
        public static int Legendary = 7;
    }
}