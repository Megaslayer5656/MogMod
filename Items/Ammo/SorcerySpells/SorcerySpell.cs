using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public abstract class SorcerySpell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo.Spells";
        public virtual int ManaCost => 1;
        public virtual int AttackSpeed => 10;
        public virtual int PlayerHurtDamage => 0;
        public virtual string PlayerDeathReason => " poured too much of their life into a sorcery.";
        public virtual SoundStyle UseSound => SoundID.Item8;
        // for spells that swing swords
        public virtual bool SwordStyle => false;
        public virtual bool Channeled => false;
        public override void SetDefaults()
        {
            // display purposes only;
            Item.mana = ManaCost;
            // so it can be used by sorcery staves;
            Item.ammo = ModContent.ItemType<GlintstonePebble>();
        }
        // replaces the "Ammo" description with "Sorcery" since i dont think you can do it in localization;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var changedLine = tooltips.FirstOrDefault(x => x.Name == "Ammo" && x.Mod == "Terraria");
            if (changedLine != null)
                changedLine.Text = "Sorcery";
        }
    }
}