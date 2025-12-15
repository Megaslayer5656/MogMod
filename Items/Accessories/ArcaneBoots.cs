using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Items.Accessories
{
    public class ArcaneBoots : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 50;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // give mana boots an on button press affect that restores 200 mana and if possible does so to everyone
            player.moveSpeed += .15f;
            player.accRunSpeed = 7f;
            player.statManaMax2 += 50;
            if (!hideVisual)
            {
                player.CancelAllBootRunVisualEffects();

                // Hellfire Treads sprint dust. For more info on sprint dusts see Player.SpawnFastRunParticles() method in Player.cs
                player.coldDash = true;
                // Other boot run visual effects include: sailDash, coldDash, desertDash, fairyBoots

                // makes a fire trail behind you on the ground
                //if (!player.mount.Active || player.mount.Type != MountID.WallOfFleshGoat)
                //{
                //    player.DoBootsEffect(player.DoBootsEffect_PlaceFlamesOnTile);
                //}
            }
        }
    }
}
