using MogMod.Items.Global;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MogMod.Rarities;

namespace MogMod.Items.Ammo.SorcerySpells
{
    public class EmptySpell : SorcerySpell
    {
        public override string Texture => "MogMod/Items/Armor/Other/PleaseStopMe";
        public override int ManaCost => 0;
        public override int AttackSpeed => 60;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 0;
            Item.width = Item.height = 24;
            Item.knockBack = 2f;
            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = (int)(MogGlobalItem.RarityVonBuyPrice * 0.05f);
            // so you cant hold onto this item (doesnt work)
            Item.stack = 0;
            SorceryClass = SorceryID.None;
        }
    }
}