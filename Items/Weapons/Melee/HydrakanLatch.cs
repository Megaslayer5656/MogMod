using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MogMod.Common.MogModPlayer;
using MogMod.Buffs.PotionBuffs;

namespace MogMod.Items.Weapons.Melee
{
    public class HydrakanLatch : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 91;
            Item.scale = .65f;
            Item.damage = 15;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 25;
            Item.useTurn = false;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.type != NPCID.TargetDummy)
            {
                player.AddBuff(ModContent.BuffType<EssenceShift>(), 600);
                MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
                mogPlayer.essenceShiftLevel += 1;
            }
        }
    }
}
