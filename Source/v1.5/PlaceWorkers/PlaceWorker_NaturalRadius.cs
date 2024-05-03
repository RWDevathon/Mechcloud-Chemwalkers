using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class PlaceWorker_Mechcloud : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            base.DrawGhost(def, center, rot, ghostCol, thing);
            float? radius = def.GetModExtension<ABF_PawnSpawnerExtension>().naturalRadius;
            if (radius != null)
            {
                MeditationUtility.DrawArtificialBuildingOverlay(center, def, Find.CurrentMap, (float)radius);

                List<Thing> animaTrees = Find.CurrentMap.listerThings.ThingsOfDef(ThingDefOf.Plant_TreeAnima);
                for (int i = animaTrees.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(animaTrees[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), animaTrees[i].TrueCenter(), SimpleColor.Green);
                    }
                }

                List<Thing> mechcloudSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_MechcloudInscribedChemwalkerShell);
                for (int i = mechcloudSpawners.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(mechcloudSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), mechcloudSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> lumberingSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_LumberingInscribedChemwalkerShell);
                for (int i = lumberingSpawners.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(lumberingSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), lumberingSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }
            }
        }

        public override void DrawPlaceMouseAttachments(float curX, ref float curY, BuildableDef def, IntVec3 center, Rot4 rot)
        {
            base.DrawPlaceMouseAttachments(curX, ref curY, def, center, rot);

            // 5% efficiency max if roofed or indoors
            if (Find.CurrentMap.roofGrid.Roofed(center) || (center.GetRoom(Find.CurrentMap) is Room room && !room.TouchesMapEdge))
            {
                Rect roofWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                string roofWarningText = "ABF_RoofPenalty".Translate();
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                GUI.color = Color.green;
                Widgets.Label(roofWarningRect, roofWarningText);
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
                return;
            }

            float? radius = def.GetModExtension<ABF_PawnSpawnerExtension>().naturalRadius;
            if (radius != null)
            {
                // 40% bonus per anima tree
                List<Thing> animaTrees = Find.CurrentMap.listerThings.ThingsOfDef(ThingDefOf.Plant_TreeAnima);
                int treesInRange = 0;
                for (int i = animaTrees.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(animaTrees[i].Position) < radius)
                    {
                        treesInRange++;
                    }
                }
                if (treesInRange > 0)
                {
                    Rect treeBonusRect = new Rect(curX, curY, 999f, Text.LineHeight);
                    string treeBonusText = "ABF_BonusPerAnimaTree".Translate(treesInRange * 40);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GUI.color = Color.green;
                    Widgets.Label(treeBonusRect, treeBonusText);
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                    curY += Text.LineHeight + Text.SpaceBetweenLines;
                }

                // 3% penalty per inscribed mechcloud shell
                List<Thing> mechcloudShells = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_MechcloudInscribedChemwalkerShell);
                int mechcloudShellsInRange = 0;
                for (int i = mechcloudShells.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(mechcloudShells[i].Position) < radius)
                    {
                        mechcloudShellsInRange++;
                    }
                }
                if (mechcloudShellsInRange > 0)
                {
                    Rect mechcloudShellWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                    string mechcloudShellWarningText = "ABF_PenaltyPerMechcloudShell".Translate(mechcloudShellsInRange * 3);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GUI.color = Color.red;
                    Widgets.Label(mechcloudShellWarningRect, mechcloudShellWarningText);
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                    curY += Text.LineHeight + Text.SpaceBetweenLines;
                }

                // 6% penalty per inscribed lumbering shell
                List<Thing> lumberingShells = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_MechcloudInscribedChemwalkerShell);
                int lumberingShellsInRange = 0;
                for (int i = lumberingShells.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(lumberingShells[i].Position) < radius)
                    {
                        lumberingShellsInRange++;
                    }
                }
                if (lumberingShellsInRange > 0)
                {
                    Rect lumberingShellWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                    string lumberingShellWarningText = "ABF_PenaltyPerLumberingShell".Translate(lumberingShellsInRange * 6);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GUI.color = Color.red;
                    Widgets.Label(lumberingShellWarningRect, lumberingShellWarningText);
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                    curY += Text.LineHeight + Text.SpaceBetweenLines;
                }

                // 2% penalty per artificial entity
                int issues = Find.CurrentMap.listerArtificialBuildingsForMeditation.GetForCell(center, (float)radius).Count;
                if (issues != 0)
                {
                    Rect artificialWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                    string artificialWarningText = "ABF_PenaltyPerArtificial".Translate(issues * 2);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GUI.color = Color.red;
                    Widgets.Label(artificialWarningRect, artificialWarningText);
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                    curY += Text.LineHeight + Text.SpaceBetweenLines;
                }
            }
        }
    }
}
