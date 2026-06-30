using Microsoft.Xna.Framework;
using MogMod.Buffs.Summons;
using MogMod.Common.MogModPlayer;
using MogMod.Items.Global;
using MogMod.Items.Other;
using MogMod.Projectiles.SummonerProjectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace MogMod.Items.Armor.FrostMaiden
{
    [AutoloadEquip(EquipType.Head)]
    public class FrostMaidenSummon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor";
        public static int CrystalDamage = 20;
        public static LocalizedText SetBonusText { get; private set; }
        public override void SetStaticDefaults()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // worn on head
            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Head);

            // set bonus text
            SetBonusText = this.GetLocalization("SetBonus");
        }
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 22;
            Item.defense = 4;
            Item.rare = ItemRarityID.Orange;
            Item.value = MogGlobalItem.RarityOrangeBuyPrice;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<FrostMaidenRobe>() && legs.type == ModContent.ItemType<FrostMaidenPants>();
        }
        public override void UpdateArmorSet(Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            mogPlayer.wearingFrostArmor = true;
            mogPlayer.wearingFrostSummon = true;
            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<FrostCrystalSummonBuff>()) == -1)
                    player.AddBuff(ModContent.BuffType<FrostCrystalSummonBuff>(), 3600, true);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<FrostCrystalSummon>()] < 1)
                {
                    var damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(CrystalDamage);
                    var p = Projectile.NewProjectileDirect(source, player.Center, -Vector2.UnitY, ModContent.ProjectileType<FrostCrystalSummon>(), damage, 0f, Main.myPlayer, 50f, 0f);
                    p.originalDamage = CrystalDamage;
                }
            }
            player.setBonus = SetBonusText.Value;
            player.maxMinions += 1;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.10f;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Bone, 20).
                AddIngredient<FrigidShard>(5).
                AddIngredient(ItemID.FlinxFur, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}