using MogMod.Items.Ammo;
using MogMod.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Global
{
    public class MogGlobalItem : GlobalItem
    {
        public int bloodDamage;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[ItemID.WizardHat] = ModContent.ItemType<GlintstoneArc>();
        }
        public override void SetDefaults(Item entity)
        {
            if (entity.type == ModContent.ItemType<Reduvia>())
            {
                bloodDamage = 33;

            } else if (entity.type == ModContent.ItemType<Sange>())
            {
                bloodDamage = 110;

            } else if (entity.type == ModContent.ItemType<RiversOfBlood>())
            {
                bloodDamage = 135;

            } else
            {
                bloodDamage = 0;

            }
        }
        public override bool InstancePerEntity => true;
    }
}
