using System.Collections;

namespace HaloScreenshots
{
    class Maps
    {
        private Hashtable mapList = new Hashtable()
        {
            // Halo 3 Campaign
            {3005, "Arrival"},
            {3010, "Sierra 117"},
            {3020, "Crow's Nest"},
            {3030, "Tsavo Highway"},
            {3040, "The Storm"},
            {3050, "Floodgate"},
            {3070, "The Ark"},
            {3100, "The Covenant"},
            {3110, "Cortana"},
            {3120, "Halo"},

            // Halo 3 Multiplayer
            {30, "Last Resort"},
            {300, "Construct"},
            {310, "High Ground"},
            {320, "Guardian"},
            {330, "Isolation"},
            {340, "Valhalla"},
            {350, "Epitaph"},
            {360, "Snowbound"},
            {380, "Narrows"},
            {390, "The Pit"},
            {400, "Sandtrap"},
            {410, "Standoff"},
            {440, "Longshore"},
            {470, "Avalanche"},
            {480, "Foundry"},
            {490, "Assembly"},
            {500, "Orbital"},
            {520, "Blackout"},
            {580, "Rat's Nest"},
            {590, "Ghost Town"},
            {600, "Cold Storage"},
            {720, "Heretic"},
            {730, "Sandbox"},
            {740, "Citadel"},

            // ODST Campaign
            {5000, "Mombasa Streets"},
            {5200, "Data Hive"},
            {5300, "Coastal Highway"},
            {6100, "Tayari Plaza"},
            {6110, "Uplift Reserve"},
            {6120, "Kizingo Blvd."},
            {6130, "ONI Alpha Site"},
            {6140, "NMPD HQ"},
            {6150, "Kikowani Station"},

            // ODST Firefight
            // Will need some extra processing to ensure proper ID is read
            {15007, "Crater (night) "},
            {15008, "Rally (night)"},
            {15206, "Chasm Ten"},
            {15307, "Last Exit"},
            {16104, "Crater"},
            {16114, "Lost Platoon"},
            {16125, "Rally Point"},
            {16134, "Security Zone"},
            {16135, "Alpha Site"},
            {16145, "Windward"},

            // Reach Beta
            // Will need extra processing to differentiate between Beta and Retail
            // Each of these ID's are 10000 more than the actual ID, but I had to keep
            // them separate from the Retail maps.
            {11000, "Sword Base"},
            {11055, "Powerhouse"},
            {11080, "Boneyard"},
            {17000, "Overlook"},

            // Halo: Reach Campaign
            {5010, "Winter Contingency"},
            {5020, "ONI: Sword Base"},
            {5030, "Nightfall"},
            {5035, "Tip of the Spear"},
            {5045, "Tip of the Spear"},
            {5050, "Exodus"},
            {5052, "New Alexandria"},
            {5060, "The Package"},
            {5070, "The Pillar of Autumn"},
            {5080, "Lone Wolf"},

            // Halo: Reach Firefight
            {7000, "Overlook"},
            {7020, "Courtyard"},
            {7030, "Outpost"},
            {7040, "Waterfront"},
            {7060, "Beachhead"},
            {7080, "Holdout"},
            {7110, "Corvette"},
            {7130, "Glacier"},
            {7500, "Unearthed"},
            {10080, "Installation 04"},

            // Halo: Reach Multiplayer
            {1000, "Sword Base"},
            {1020, "Countdown"},
            {1035, "Boardwalk"},
            {1040, "Zealot"},
            {1055, "Powerhouse"},
            {1080, "Boneyard"},
            {1150, "Reflection"},
            {1200, "Spire"},
            {1500, "Condemned"},
            {1510, "Highlands"},
            {2001, "Anchor 9"},
            {2002, "Breakpoint"},
            {2004, "Tempest"},
            {3006, "Forge World"},
            {10010, "Penance"},
            {10020, "Battle Canyon"},
            {10030, "Ridgeline"},
            {10050, "Breakneck"},
            {10060, "High Noon"},
            {10070, "Solitary"},

            // Halo 4
            {10081, "Haven"}, // The actual mapId is 10080, add 1 when gameid = 4 && mapid == 10080 to avoid discrepancy with Halo: Reach's "Installation 04"
            {10085, "Complex"},
            {10091, "Ragnarok"},
            {10200, "Longbow"},
            {10202, "Solace"},
            {10210, "Adrift"},
            {10225, "Abandon"},
            {10226, "Exile"},
            {10245, "Erosion"},
            {10252, "Vortex"},
            {10255, "Impact"},
            {10256, "Ravine"},
            {10261, "Meltdown"},
        };

        public string getMapName(int id)
        {
            return (string)mapList[id];
        }
    }
}