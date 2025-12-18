using System;
using Terraria.ModLoader;
using Terraria.ID;

namespace MogMod.Items.Placeable
{
    public abstract class MusicBox : ModItem
    {
        public abstract int MusicBoxTile { get; }
        public abstract string MusicFilePath { get; }
        public virtual bool Obtainable { get; } = true;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.CanGetPrefixes[Type] = false;
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.MusicBox;

            if (!Obtainable)
            {
                Item.ResearchUnlockCount = 0;
            }

            // Register Music Box if the music path exists
            if (!String.IsNullOrEmpty(MusicFilePath))
                MusicLoader.AddMusicBox(Mod, MusicLoader.GetMusicSlot(Mod, MusicFilePath), Type, MusicBoxTile);
        }

        public override void SetDefaults()
        {
            Item.DefaultToMusicBox(MusicBoxTile, 0);
        }
    }
}