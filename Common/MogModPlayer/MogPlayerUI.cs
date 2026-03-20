using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MogMod.Common.MogModPlayer
{
    // taken from example mod
    public class MogPlayerUI : ModPlayer
    {
        // Here we create a custom resource, similar to mana or health.
        // Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
        public int gunlanceCurrent; // Current value of our example resource
        public const int gunlanceMax = 3; // Default maximum value of example resource
        public int exampleResourceMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().
        public int exampleResourceMax2; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource
        public float exampleResourceRegenRate; // By changing that variable we can increase/decrease regeneration rate of our resource
        internal int exampleResourceRegenTimer = 0; // A variable that is required for our timer

        // In order to make the Example Resource example straightforward, several things have been left out that would be needed for a fully functional resource similar to mana and health.
        // Here are additional things you might need to implement if you intend to make a custom resource:
        // - Multiplayer Syncing: The current example doesn't require MP code, but pretty much any additional functionality will require this. ModPlayer.SendClientChanges and CopyClientState will be necessary, as well as SyncPlayer if you allow the user to increase exampleResourceMax.
        // - Save/Load permanent changes to max resource: You'll need to implement Save/Load to remember increases to your exampleResourceMax cap.

        public override void Initialize()
        {
            exampleResourceMax = gunlanceMax;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            ResetVariables();
        }

        // We need this to ensure that regeneration rate and maximum amount are reset to default values after increasing when conditions are no longer satisfied (e.g. we unequip an accessory that increases our resource)
        private void ResetVariables()
        {
            exampleResourceRegenRate = 1f;
            exampleResourceMax2 = exampleResourceMax;
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }

        public override void PostUpdate()
        {
            CapResourceGodMode();
        }

        // Lets do all our logic for the custom resource here, such as limiting it, increasing it and so on.
        private void UpdateResource()
        {
            // Limit gunlanceCurrent from going over the limit imposed by exampleResourceMax.
            gunlanceCurrent = Utils.Clamp(gunlanceCurrent, 0, exampleResourceMax2);
        }

        private void CapResourceGodMode()
        {
            if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode)
            {
                gunlanceCurrent = exampleResourceMax2;
            }
        }
    }
}