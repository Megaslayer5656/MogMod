using MogMod.Common.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MogMod.Common.Systems
{
    public class DragonInstallScene : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/RideTheFire"); //Plays dragon install music

        public override bool IsSceneEffectActive(Terraria.Player player)
        {
            MogPlayer mogPlayer = player.GetModPlayer<MogPlayer>();
            return mogPlayer.dragonInstallActive; //Sets the scene to active if a player is dragon installed
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        //public override void SpecialVisuals(Terraria.Player player, bool isActive)
        //{
        //    base.SpecialVisuals(player, isActive);
        //}

        //TODO: Add special dragon install visuals
    }
}
