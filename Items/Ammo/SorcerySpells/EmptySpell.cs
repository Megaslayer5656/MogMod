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
        public override string Texture => "MogMod/Projectiles/BaseProjectiles/InvisibleProj";
        public override int ManaCost => 0;
        public override int AttackSpeed => 60;
        public override SoundStyle UseSound => SoundID.Item9;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 0;
            Item.DamageType = DamageClass.Magic;
            Item.width = Item.height = 36;
            Item.knockBack = 2f;
            Item.rare = ModContent.RarityType<VonRarity>();
            Item.value = MogGlobalItem.RarityVonBuyPrice;
            // so you cant hold onto this item
            Item.stack = 0;
        }
    }
}