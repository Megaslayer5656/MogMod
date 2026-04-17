using Microsoft.Xna.Framework;
using MogMod.Items.Consumables;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Common.MogModPlayer
{
    public class MogFishingPlayer : ModPlayer
    {
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            if (inWater && Player.ZoneBeach && !attempt.crate)
            {
                // If the game rolls a crate, we want to give ours to the player if he is in Example Surface Biome

                // We don't want to replace golden/titanium crates (the highest tier crates), as they take highest priority in crate catches
                // Their drop conditions are "veryrare" or "legendary"
                // (After that come biome crates ("rare"), then iron/mythril ("uncommon"), then wood/pearl (none of the previous))
                // Let's replace biome crates 50% of the time (player could be in multiple (modded) biomes, we should respect that)
                if (!attempt.legendary && !attempt.veryrare && attempt.rare && Main.rand.NextBool(3))
                {
                    itemDrop = ModContent.ItemType<AnglerFish>();
                    return; // This is important so your code after this that rolls items will not run
                }
            }
            if (inWater && Player.ZoneRockLayerHeight && !attempt.crate)
            {
                if (!attempt.legendary && !attempt.veryrare && attempt.rare && Main.rand.NextBool(3))
                {
                    itemDrop = ItemID.Geode;
                    return;
                }
            }
        }
    }
}