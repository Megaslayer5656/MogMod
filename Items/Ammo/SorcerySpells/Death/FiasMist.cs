using MogMod.Items.Global;
using MogMod.Projectiles.MagicProjectiles.Sorceries;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Ammo.SorcerySpells.Death
{
    public class FiasMist : SorcerySpell
    {
        public override int ManaCost => 18;
        public override int AttackSpeed => 40;
        public override SoundStyle UseSound => SoundID.Item34;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 36;
            Item.damage = -1;
            Item.knockBack = 0f;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
            Item.shoot = ModContent.ProjectileType<FiasMistProj>();
            Item.shootSpeed = 4f;
            SorceryClass = SorceryID.Death;
        }
        public override bool? UseItem(Player player)
        {
            foreach (Projectile proj in Main.projectile)
                if (proj.active && proj.owner == Main.myPlayer && proj.type == Item.shoot)
                    proj.Kill();
            return true;
        }
    }
}