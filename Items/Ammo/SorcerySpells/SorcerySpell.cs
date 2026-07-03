using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public abstract class SorcerySpell : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo.Spells";
        public virtual int ManaCost => 1;
        public virtual int AttackSpeed => 10;
        public virtual SoundStyle UseSound => SoundID.Item8;
        // for spells like carian slicer
        public virtual bool SwordStyle => false;
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