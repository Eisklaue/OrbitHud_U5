using System;
using UnityEngine;

namespace OrbitHud_U5
{
    [KSPAddon (KSPAddon.Startup.Flight, false)]
    public class OrbitHud : MonoBehaviour
    {
        // These objects hold the stats we're preparing for display
        //private DisplayItem period = new DisplayItem("Period:");
        //private DisplayItem inclination = new DisplayItem("Inclination:");
        private DisplayItem apoapsis = new DisplayItem("Apoapsis:");
        private DisplayItem periapsis = new DisplayItem("Periapsis:");
        //private DisplayItem altitude = new DisplayItem("Altitude (sea level):");
        private DisplayItem altitudeR = new DisplayItem("Altitude:");
        private DisplayItem biome = new DisplayItem("Biome:");

        // The screen boundaries we print everything to
        private static float left = (Screen.width / 2) - 600;
        private static float top = Screen.height - 135;
        private static float width = 360;
        private static float height = 105;

        // the level of transparency from 0 (see through) to 1 (opaque)
        private static float transparency = 0.6f;

        // the screen object we print to
        private static Rect layoutArea = new Rect(left, top, width, height);

        // the background texture
        private static bool bgBuilt = false;
        private static Texture2D bgTex = new Texture2D((int)width, (int)height);

        // whether or not to show the GUI (toggles with F2 key pressing)
        private bool showGUI = true;

        // public hook called by KSP once at game/scene starts
        public void Start()
        {
            buildBackground();
        }

        // public hook called by KSP when things change we need to know about (i think)
        public void Update()
        {

            if (Input.GetKeyDown(KeyCode.F2))
            {
                showGUI = !showGUI;
            }

        }


        // public hook called by KSP at regular intervals to do physics calculations etc
        // we take this oportunity to recalculate our items for display
        public void FixedUpdate()
        {
            // only bother if a ship is actually active
            if (FlightGlobals.ActiveVessel != null)
            {
                Orbit o = FlightGlobals.ActiveVessel.orbit;
                prepareApoapsis(o);
                preparePeriapsis(o);
                prepareBiome();

            }
        }

        // Public hook called by KSP every time the GUI is re-rendered
        public void OnGUI()
        {
            if (FlightGlobals.ActiveVessel != null && showGUI)
            {
                layoutArea = GUILayout.Window(1004, layoutArea, paintWindow, "Orbit Hud");
            }
        }

        void paintWindow(int windowID)
        {
            GUILayout.BeginVertical();
            printDisplayItem(apoapsis);
            printDisplayItem(periapsis);
            printDisplayItem(biome);
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void prepareBiome()
        {

            CBAttributeMapSO.MapAttribute mapAttribute;

            try
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                CBAttributeMapSO BiomeMap = vessel.mainBody.BiomeMap;
                double lat = vessel.latitude * Math.PI / 180d;
                double lon = vessel.longitude * Math.PI / 180d;
                mapAttribute = BiomeMap.GetAtt(lat, lon);
            }
            catch (NullReferenceException)
            {
                mapAttribute = new CBAttributeMapSO.MapAttribute();
                mapAttribute.name = "N/A";
            }

            this.biome.value = mapAttribute.name;

        }

        private void prepareApoapsis(Orbit o)
        {
            string timeToAp = formatTimespan(TimeSpan.FromSeconds(o.timeToAp));
            this.apoapsis.value = Math.Floor(o.ApA).ToString("#,##0") + "m (" + timeToAp + ")";
        }

        private void preparePeriapsis(Orbit o)
        {
            string timeToPer = formatTimespan(TimeSpan.FromSeconds(o.timeToPe));
            if (o.PeA < 0)
            {
                this.periapsis.value = "0m (" + timeToPer + ")";
            }else {
                this.periapsis.value = Math.Floor(o.PeA).ToString("#,##0") + "m (" + timeToPer + ")";
            }
            
        }

        private string formatTimespan(TimeSpan t)
        {

            if (t.TotalDays >= 2)
            {
                // 15d 13h
                return string.Format("{0:D2}d {1:D2}h", t.Days, t.Hours);
            }

            if (t.TotalHours >= 1)
            {
                // 13h 20min 23.123s
                return string.Format("{0:D1}h {1:D1}min {2:D1}.{3:D1}s", t.Hours, t.Minutes, t.Seconds, t.Milliseconds / 100);
            }

            if (t.TotalMinutes >= 1)
            {
                // 13h 20min 23.123s
                return string.Format("{0:D1}min {1:D1}.{2:D1}s", t.Minutes, t.Seconds, t.Milliseconds / 100);
            }

            // 13h 20min 23.123s
            return string.Format("{0:D1}.{1:D1}s", t.Seconds, t.Milliseconds / 100);

        }

        // a helper that re-paints a line in the dispaly area
        private void printDisplayItem(DisplayItem item)
        {
            int divider = 125; // the point on screen to stop column one and start column two
            GUILayoutOption colA = GUILayout.Width(divider);
            GUILayoutOption colB = GUILayout.Width(width - divider);
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.label, item.labelStyle, colA);
            GUILayout.Label(item.value, item.valueStyle, colB);
            GUILayout.EndHorizontal();
        }

        private void buildBackground()
        {

            // don't do this more than once, it's not needed
            if (bgBuilt)
            {
                return;
            }

            bgBuilt = true;
            Color c = Color.black;
            c.a = transparency;

            for (int x = 0; x <= width; x++)
            {
                for (int y = 0; y <= height; y++)
                {
                    bgTex.SetPixel(x, y, c);
                }
            }

            bgTex.Apply();

        }

    }
}
