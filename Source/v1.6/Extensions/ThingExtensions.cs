using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class ABF_PawnSpawnerExtension : DefModExtension
    {
        public PawnKindDef pawnToSpawnSuccess;

        public PawnKindDef pawnToSpawnFailure;

        public int baseTicksBetweenSpawns = -1;

        public float? naturalRadius = null;

        public bool destroyOnSpawn;

        public override IEnumerable<string> ConfigErrors()
        {
            if (pawnToSpawnSuccess == null)
            {
                yield return "ABF_PawnSpawnerExtension pawnToSpawnSuccess must be given a PawnKindDef to spawn on success.";
            }
            if (baseTicksBetweenSpawns == -1)
            {
                yield return "ABF_PawnSpawnerExtension baseTicksBetweenSpawns must be given an int representing how many ticks between spawns.";
            }
        }
    }
}
