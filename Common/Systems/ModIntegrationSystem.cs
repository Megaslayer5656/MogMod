using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MogMod.NPCs.Bosses;
using Terraria.ModLoader;
using MogMod.Items.Consumables;

namespace MogMod.Common.Systems
{
    internal class ModIntegrationSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly

            // Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
            // https://forums.terraria.org/index.php?threads/.50668/
            DoBossChecklistIntegration();

            // We can integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
        }

        private void DoBossChecklistIntegration()
        {
            // The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
            // If we navigate the wiki, we can find the "LogBoss" method, which we want in this case
            // A feature of the call is that it will create an entry in the localization file of the specified NPC type for its spawn info, so make sure to visit the localization file after your mod runs once to edit it

            if (!ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                return;
            }

            // For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.6
            // Usually mods either provide that information themselves in some way, or it's found on the GitHub through commit history/blame
            if (bossChecklistMod.Version < new Version(1, 6))
            {
                return;
            }

            // The "LogBoss" method requires many parameters, defined separately below:

            // Your entry key can be used by other developers to submit mod-collaborative data to your entry. It should not be changed once defined
            string internalName = "Dabdad";

            // Value inferred from boss progression, see the wiki for details
            float weight = 0.7f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedDabDad;

            // The NPC type of the boss
            int bossType = ModContent.NPCType<DabDad>();

            // The item used to summon the boss with (if available)
            int spawnItem = ModContent.ItemType<DabDadSpawner>();

            // "collectibles" like relic, trophy, mask, pet
            List<int> collectibles = new List<int>()
            {
            };
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = spawnItem,
                    ["collectibles"] = collectibles
                    // Other optional arguments as needed are inferred from the wiki
                }
            );

            // Other bosses or additional Mod.Call can be made here.
        }
    }
}
