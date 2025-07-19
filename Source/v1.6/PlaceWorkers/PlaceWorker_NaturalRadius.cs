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

                List<Thing> mechcloudSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MechcloudInscribed);
                for (int i = mechcloudSpawners.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(mechcloudSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), mechcloudSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> lumberingSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_LumberingInscribed);
                for (int i = lumberingSpawners.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(lumberingSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), lumberingSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> flakkerSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_FlakkerInscribed);
                for (int i = flakkerSpawners.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(flakkerSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), flakkerSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> marineSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MarineInscribed);
                for (int i = marineSpawners.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(marineSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), marineSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }

                List<Thing> cataphractSpawners = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_CataphractInscribed);
                for (int i = cataphractSpawners.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(cataphractSpawners[i].Position) <= radius)
                    {
                        GenDraw.DrawLineBetween(GenThing.TrueCenter(center, Rot4.North, def.size, def.Altitude), cataphractSpawners[i].TrueCenter(), SimpleColor.Red);
                    }
                }
            }
        }

        public override void DrawPlaceMouseAttachments(float curX, ref float curY, BuildableDef def, IntVec3 center, Rot4 rot)
        {
            base.DrawPlaceMouseAttachments(curX, ref curY, def, center, rot);

            // 5% efficiency max if roofed or indoors
            if (Find.CurrentMap.roofGrid.Roofed(center) || (center.GetRoom(Find.CurrentMap) is Room room && !room.PsychologicallyOutdoors))
            {
                Rect roofWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                string roofWarningText = "ABF_RoofPenalty".Translate();
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                GUI.color = Color.red;
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
                List<Thing> mechcloudShells = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MechcloudInscribed);
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
                List<Thing> lumberingShells = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_LumberingInscribed);
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

                // 4% penalty per inscribed flakker shell
                List<Thing> flakkerShells = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_FlakkerInscribed);
                int flakkerShellsInRange = 0;
                for (int i = flakkerShells.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(flakkerShells[i].Position) < radius)
                    {
                        flakkerShellsInRange++;
                    }
                }
                if (flakkerShellsInRange > 0)
                {
                    Rect flakkerShellWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                    string flakkerShellWarningText = "ABF_PenaltyPerFlakkerShell".Translate(flakkerShellsInRange * 4);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GUI.color = Color.red;
                    Widgets.Label(flakkerShellWarningRect, flakkerShellWarningText);
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                    curY += Text.LineHeight + Text.SpaceBetweenLines;
                }

                // 5% penalty per inscribed marine shell
                List<Thing> marineShells = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_MarineInscribed);
                int marineShellsInRange = 0;
                for (int i = marineShells.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(marineShells[i].Position) < radius)
                    {
                        marineShellsInRange++;
                    }
                }
                if (marineShellsInRange > 0)
                {
                    Rect marineShellWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                    string marineShellWarningText = "ABF_PenaltyPerMarineShell".Translate(marineShellsInRange * 5);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GUI.color = Color.red;
                    Widgets.Label(marineShellWarningRect, marineShellWarningText);
                    Text.Anchor = TextAnchor.UpperLeft;
                    GUI.color = Color.white;
                    curY += Text.LineHeight + Text.SpaceBetweenLines;
                }

                // 8% penalty per inscribed cataphract shell
                List<Thing> cataphractShells = Find.CurrentMap.listerThings.ThingsOfDef(ABF_ThingDefOf.ABF_Thing_Chemwalker_Shell_CataphractInscribed);
                int cataphractShellsInRange = 0;
                for (int i = cataphractShells.Count - 1; i >= 0; i--)
                {
                    if (center.DistanceTo(cataphractShells[i].Position) < radius)
                    {
                        cataphractShellsInRange++;
                    }
                }
                if (cataphractShellsInRange > 0)
                {
                    Rect cataphractShellWarningRect = new Rect(curX, curY, 999f, Text.LineHeight);
                    string cataphractShellWarningText = "ABF_PenaltyPerCataphractShell".Translate(cataphractShellsInRange * 8);
                    Text.Font = GameFont.Small;
                    Text.Anchor = TextAnchor.MiddleLeft;
                    GUI.color = Color.red;
                    Widgets.Label(cataphractShellWarningRect, cataphractShellWarningText);
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
