using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.WorldBuilding;

namespace MogMod.Items.Weapons.Magic
{
    // 4 spell slots that each can contain a spell
    // left click fires current spell and discards it, if empty, nothing happens
    // right click bookmarks a spell if the spell slot is empty, switches current spell slot otherwise
    // different spells interact differently with eachother (for more interesting damage potential)
    // spell slots have ui elements && each spell has a unique ui element, refer to gunlance ui for help
    // "Does meaning have a meaning"
    // 34x40
    // post moon-lord
    /*Spell List
     * * = proj sprited
     * # = dust effect
     * @ = copied texture
     * ! = ui sprited
     * OFFENSIVE SPELLS:
     * rotating water block (travels in a straight line) *
     * lingering flame (circle of flame that comes to a stop) *
     * 3 fast moving thunder swords (similar to sky fracture / kaya (use kaya proj texture)) @
     * slow moving gravity magic orb (slowly travels in a straight line) *
     * ice spikes (rises from the ground (refer to calamity mod hematemesis))
     * void explosion (circle around you that deals damage after 3 seconds) #
     * 
     * PLAYER SPELLS (spells that modify the player):
     * defense shield (+ 15 defense) #
     * health regen (overtime / instant) #
     * mana regen (overtime / instant) #
     * movement boost (increased movement speed, jump height, && wing time) #
     * teleport (teleports to cursor (similar to blink dagger)) #
     * 
     * SLOT SPELLS:
     * full bookmark (bookmarks all empty spell slots) #
     * auto bookmark (auto bookmarks after firing a spell) #
     * shuffle bookmark (randomizes current spells) #
     * replay spell (current spell fired does not get discarded) #
     * 
     * STAFFS:
     * speed rod (+extra updates to all spells)
     * slow rod (-extra updates to all spells)
     * gravity rod (pulls in spells)
     * repulsion rod (pushes away spells)
     */
    internal class TheGravity
    {
    }
}