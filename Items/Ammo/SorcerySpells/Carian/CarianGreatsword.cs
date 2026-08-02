using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Carian
{
    public class CarianGreatsword : SorcerySpell
    {
        public override int ManaCost => 20;
        public override int AttackSpeed => 50;
        public override SoundStyle UseSound => SoundID.Item9;
        public override bool SwordStyle => true;
        public override bool OnlyOneActive => true;
        public override bool Channeled => true;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = 202;
            Item.knockBack = 12f;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = MogGlobalItem.RarityLightPurpleBuyPrice;
            Item.shoot = ModContent.ProjectileType<CarianGreatswordHoldout>();
            Item.shootSpeed = 8f;
            SorceryClass = SorceryID.Carian;
        }
    }
}