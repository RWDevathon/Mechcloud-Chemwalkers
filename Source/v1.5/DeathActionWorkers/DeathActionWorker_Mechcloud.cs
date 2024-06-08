using Verse;
using Verse.AI.Group;

namespace ArtificialBeings
{
    // This DeathActionWorker exists to be picked up by a harmony patch to kill downed Chemwalkers of non-player factions.
    public class DeathActionWorker_Mechcloud : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse, Lord lord)
        {
        }
    }
}
