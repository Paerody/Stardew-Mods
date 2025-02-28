using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using StardewValley.Monsters;
using xTile.Dimensions;
using StardewValley.Objects;
using System.Threading;
using StardewModdingAPI.Utilities;

namespace NapalmMummies
{
    public partial class ModEntry
    {

        [HarmonyPatch(typeof(Mummy), nameof(Mummy.takeDamage))]
        public class Mummy_takeDamage_Patch
        {
            public static void Postfix(Mummy __instance, Farmer who)
            {
                if (!Config.ModEnabled || __instance.reviveTimer.Value == 0)
                    return;
                List<Ring> rings = GetAllSplitRings(who);
                
                if (rings.Exists(r => r is not null && r.ItemId == "811"))
                    __instance.currentLocation.explode(__instance.Tile, 2, who, false, -1);
            }

            private static List<Ring> GetAllSplitRings(Farmer who)
            {
                List<Ring> rings = new List<Ring>();
                if (WearMoreRingsAPI_2 != null)
                {
                    for (int i = 0; i < WearMoreRingsAPI_2.Value.RingSlotCount(); i++)
                    {
                        if (WearMoreRingsAPI_2.Value.GetRing(i) is CombinedRing)
                        {
                            rings.AddRange((WearMoreRingsAPI_2.Value.GetRing(i) as CombinedRing).combinedRings);
                        }
                        else
                        {
                            rings.Add(WearMoreRingsAPI_2.Value.GetRing(i));
                        }
                        rings.Add(WearMoreRingsAPI_2.Value.GetRing(i));
                    }    
                }
                else
                {
                    if (who.leftRing.Value is CombinedRing)
                    {
                        rings.AddRange((who.leftRing.Value as CombinedRing).combinedRings);
                    }
                    else
                    {
                        rings.Add(who.leftRing.Value);
                    }
                    if (who.rightRing.Value is CombinedRing)
                    {
                        rings.AddRange((who.rightRing.Value as CombinedRing).combinedRings);
                    }
                    else
                    {
                        rings.Add(who.rightRing.Value);
                    }
                }
                return rings;
            }
        }
        [HarmonyPatch(typeof(Ring), nameof(Ring.onMonsterSlay))]
        public class Ring_onMonsterSlay_Patch
        {
            public static bool Prefix(Ring __instance, Monster monster, GameLocation location, Farmer who)
            {
                // When we kill a mummy, don't run the normal Napalm Ring trigger
                if (!Config.ModEnabled || monster is not Mummy || __instance.ItemId != "811")
                    return true;
                return false;
            }
        }
    }
}