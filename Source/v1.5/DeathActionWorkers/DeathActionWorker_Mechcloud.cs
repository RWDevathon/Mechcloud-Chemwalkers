using Verse;
using Verse.AI.Group;

namespace ArtificialBeings
{
    public class DeathActionWorker_Mechcloud : DeathActionWorker
    {
        public override void PawnDied(Corpse corpse, Lord lord)
        {
            IntVec3 position = corpse.Position;
            Map map = corpse.Map;
            ThingDef thingDef = corpse.InnerPawn.def.butcherProducts.FirstOrFallback().thingDef;
            corpse.Destroy();
            Thing thing = ThingMaker.MakeThing(thingDef);
            GenSpawn.Spawn(thing, position, map);
        }
    }
}
