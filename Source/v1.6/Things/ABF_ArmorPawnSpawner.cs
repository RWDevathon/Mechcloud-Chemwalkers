using RimWorld;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class ABF_ArmorPawnSpawner : ThingWithComps
    {
        private ABF_PawnSpawnerExtension spawnerExtension;

        public float workRemaining = -1f;

        private float workFactor = 1f;

        private bool readyForAttunement = false;

        public virtual bool Active => Spawned;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            spawnerExtension = def.GetModExtension<ABF_PawnSpawnerExtension>();

            if (workRemaining == -1f)
            {
                ResetWorkRemaining();
            }
            RecalculateFactor();
            readyForAttunement = false;
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption option in base.GetFloatMenuOptions(selPawn))
            {
                yield return option;
            }

            if (workRemaining <= 0)
            {
                if (selPawn.health.hediffSet.HasHediff(HediffDefOf.PsychicAmplifier) && selPawn.GetStatValue(StatDefOf.PsychicSensitivity, cacheStaleAfterTicks: 6000) > 0)
                {
                    yield return new FloatMenuOption("ABF_AttuneToArmor".Translate(), delegate ()
                    {
                        Job job = new Job(ABF_JobDefOf.ABF_Job_Chemwalker_AttuneToArmor, new LocalTargetInfo(this));
                        selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    });
                }
                else
                {
                    yield return new FloatMenuOption("ABF_RequiresPsychicPowers".Translate(selPawn.LabelShort), null);
                }
            }
            else
            {
                yield return new FloatMenuOption("ABF_ArmorNotReady".Translate(), null);
            }

            // Always allow players to "uninstall" the armor pile.
            yield return new FloatMenuOption("ABF_PackUpArmor".Translate(), delegate ()
            {
                Job job = new Job(ABF_JobDefOf.ABF_Job_Chemwalker_PackUpArmor, new LocalTargetInfo(this));
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });

            if (DebugSettings.ShowDevGizmos)
            {
                yield return new FloatMenuOption("DEV: Force complete", delegate ()
                {
                    workRemaining = 1f;
                });
            }
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            if (readyForAttunement)
            {
                return;
            }

            float? radius = spawnerExtension.naturalRadius;
            if (radius != null)
            {
                MeditationUtility.DrawArtificialBuildingOverlay(Position, def, Map, (float)radius);

                List<Thing> animaTrees = Map.listerThings.ThingsOfDef(ThingDefOf.Plant_TreeAnima);
                for (int i = animaTrees.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(animaTrees[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(Position, Rot4.North, def.size, def.Altitude), animaTrees[i].TrueCenter(), SimpleColor.Green);
                    }
                }

                List<Thing> mechcloudSpawners = Map.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MechcloudInscribed);
                for (int i = mechcloudSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(mechcloudSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(Position, Rot4.North, def.size, def.Altitude), mechcloudSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> lumberingSpawners = Map.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_LumberingInscribed);
                for (int i = lumberingSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(lumberingSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(Position, Rot4.North, def.size, def.Altitude), lumberingSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> flakkerSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_FlakkerInscribed);
                for (int i = flakkerSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(flakkerSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(Position, Rot4.North, def.size, def.Altitude), flakkerSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> marineSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MarineInscribed);
                for (int i = marineSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(marineSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(Position, Rot4.North, def.size, def.Altitude), marineSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> cataphractSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_CataphractInscribed);
                for (int i = cataphractSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(cataphractSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(Position, Rot4.North, def.size, def.Altitude), cataphractSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }
            }
        }

        protected override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (Active)
            {
                RecalculateFactor();
                workRemaining -= delta * workFactor;

                if (workRemaining <= 0 && !readyForAttunement)
                {
                    readyForAttunement = true;
                    Letter notification = LetterMaker.MakeLetter("ABF_SpawnerReadyLabel".Translate(), "ABF_SpawnerReadyDescription".Translate(def.label), LetterDefOf.PositiveEvent, this);
                    Find.LetterStack.ReceiveLetter(notification);
                }
            }
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder(base.GetInspectString());
            stringBuilder.AppendLine();

            if (!Spawned)
            {
                stringBuilder.Append("ABF_RequiresPlacement".Translate());
                return stringBuilder.ToString();
            }

            if (workRemaining > 0)
            {
                stringBuilder.AppendLine("ABF_EstimatedTimeLeft".Translate(EstimatedTicksRemaining().ToStringTicksToPeriod()));
                stringBuilder.Append("ABF_CurrentAnimationWorkFactor".Translate(workFactor.ToStringPercent()));
            }
            else
            {
                stringBuilder.Append("ABF_ReadyForAttunement".Translate());
            }
            return stringBuilder.ToString();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref workRemaining, "ABF_armorPawnWorkRemaining", -1);
            Scribe_Values.Look(ref readyForAttunement, "ABF_armorPawnReadyToSpawn", false);
        }

        public virtual void SpawnPawn(Faction ownerFaction)
        {
            if (!Spawned)
            {
                Log.Warning("[ABF] Attempted to spawn a pawn from a spawner which wasn't spawned itself!");
                return;
            }

            PawnKindDef pawnKindToSpawn;
            if (ownerFaction == null)
            {
                pawnKindToSpawn = spawnerExtension.pawnToSpawnFailure ?? spawnerExtension.pawnToSpawnSuccess;
            }
            else
            {
                pawnKindToSpawn = spawnerExtension.pawnToSpawnSuccess;
            }

            PawnGenerationRequest request = new PawnGenerationRequest(pawnKindToSpawn, ownerFaction, PawnGenerationContext.NonPlayer, forceGenerateNewPawn: true, canGeneratePawnRelations: false, allowFood: false, allowAddictions: false, fixedBiologicalAge: 0, fixedChronologicalAge: 0, fixedIdeo: null, forceNoIdeo: true, forceBaselinerChance: 1f);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            GenSpawn.Spawn(pawn, Position, Map);

            if (spawnerExtension.destroyOnSpawn)
            {
                Destroy();
            }
            else
            {
                ResetWorkRemaining();
            }
        }

        public virtual void RecalculateFactor()
        {
            workFactor = 1f;

            if (!Spawned)
            {
                return;
            }

            // Only chemwalkers that have a natural effect radius should be affected by interior/exterior conditions.
            float? radius = spawnerExtension.naturalRadius;
            if (radius != null && (Position.Roofed(Map) || (Position.GetRoom(Map) is Room room && !room.TouchesMapEdge)))
            {
                workFactor = 0.05f;
                return;
            }

            CompQuality compQuality = GetComp<CompQuality>();
            if (compQuality != null)
            {
                QualityCategory quality = compQuality.Quality;
                switch (quality)
                {
                    case QualityCategory.Awful:
                        workFactor -= 0.4f;
                        break;
                    case QualityCategory.Poor:
                        workFactor -= 0.2f;
                        break;
                    case QualityCategory.Good:
                        workFactor += 0.1f;
                        break;
                    case QualityCategory.Excellent:
                        workFactor += 0.25f;
                        break;
                    case QualityCategory.Masterwork:
                        workFactor += 0.4f;
                        break;
                    case QualityCategory.Legendary:
                        workFactor += 0.6f;
                        break;
                }
            }

            if (radius != null)
            {
                int artificialThingsNearby = Map.listerArtificialBuildingsForMeditation.GetForCell(Position, (float)radius).Count;
                workFactor -= Mathf.Max(0, 0.02f * artificialThingsNearby);

                List<Thing> animaTrees = Map.listerThings.ThingsOfDef(ThingDefOf.Plant_TreeAnima);
                for (int i = animaTrees.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(animaTrees[i].Position) <= radius)
                    {
                        workFactor += 0.4f;
                    }
                }

                List<Thing> mechcloudSpawners = Map.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MechcloudInscribed);
                for (int i = mechcloudSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(mechcloudSpawners[i].Position) <= radius)
                    {
                        workFactor -= 0.03f;
                    }
                }

                List<Thing> lumberingSpawners = Map.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_LumberingInscribed);
                for (int i = lumberingSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(lumberingSpawners[i].Position) <= radius)
                    {
                        workFactor -= 0.06f;
                    }
                }

                List<Thing> flakkerSpawners = Map.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_FlakkerInscribed);
                for (int i = flakkerSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(flakkerSpawners[i].Position) <= radius)
                    {
                        workFactor -= 0.04f;
                    }
                }

                List<Thing> marineSpawners = Map.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MarineInscribed);
                for (int i = marineSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(marineSpawners[i].Position) <= radius)
                    {
                        workFactor -= 0.04f;
                    }
                }

                List<Thing> cataphractSpawners = Map.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_CataphractInscribed);
                for (int i = cataphractSpawners.Count - 1; i >= 0; i--)
                {
                    if (Position.DistanceTo(cataphractSpawners[i].Position) <= radius)
                    {
                        workFactor -= 0.08f;
                    }
                }
            }
            workFactor = Mathf.Clamp(workFactor, 0f, 10f);
        }

        public virtual int EstimatedTicksRemaining()
        {
            return Math.Max(0, (int)(workRemaining / workFactor));
        }

        public void ResetWorkRemaining()
        {
            workRemaining = Rand.Range(spawnerExtension.baseTicksBetweenSpawns * 0.75f, spawnerExtension.baseTicksBetweenSpawns * 1.25f);
            readyForAttunement = false;
        }
    }
}
