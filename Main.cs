using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;
using System.Threading;
using System.Reflection;
using HarmonyLib;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FreeAngleHelper
{
    public class ByAttribute : Attribute { public ByAttribute(string site, params Type[] used) { } }
    public class Main
    {
        internal static Gen g;
        public static Harmony harmony;

        public static GameObject gen;
        internal static Settings set { get; set; } = new Settings();
        internal static UnityModManager.ModEntry modEntry;
        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            set = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Main.modEntry = modEntry;
            modEntry.OnToggle = (Entry, value) =>
            {
                if (value)
                {
                    harmony = new Harmony(modEntry.Info.Id);
                    harmony.PatchAll(Assembly.GetExecutingAssembly());
                    gen = new GameObject();
                    g = gen.AddComponent<Gen>();
                    ContentSizeFitter fitter = gen.AddComponent<ContentSizeFitter>();
                    fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    UnityEngine.Object.DontDestroyOnLoad(gen);
                }
                else
                {
                    harmony.UnpatchAll();
                    harmony = null;
                    UnityEngine.Object.Destroy(gen);
                    gen = null;
                    g = null;
                }
                return true;
            };
            modEntry.OnGUI = (Entry) =>
            {
                set.Draw(Entry);
                if (set.list)
                {
                    for(int i = 0; i < set.types.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if(GUILayout.Button(set.types[i].ToString()))
                        {
                            set.types.RemoveAt(i);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                } 
            };
            modEntry.OnSaveGUI = (Entry) =>
            {
                set.Save(Entry);
            };
            return true;
        }
    }
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        public void OnChange()
        {
            if (!Main.set.types.Contains(etype) && etype != ADOFAI.LevelEventType.None)
            {
                Main.set.types.Add(etype);
            }
            Save(Main.modEntry);
        }
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
        [Draw("")]
        public KeyBindSettings KeyBindSettings = new KeyBindSettings();
        //[Draw("pulseFloorButtons")]
        public bool pulseFloorButtons = true;
        //[Draw("fullSpin")]
        public bool fullSpin = false;
        [Draw("Add AddLevelEvents", DrawType.PopupList)]
        public ADOFAI.LevelEventType etype = ADOFAI.LevelEventType.None;
        [Draw("EventType List")]
        public bool list = false;
        public List<ADOFAI.LevelEventType> types = new List<ADOFAI.LevelEventType>();
        public float angle = 0;
        public float min = 0;
        public float max = 360;
    }
    public class Gen : MonoBehaviour
    {
        public GUIStyle style;
        public void Awake()
        {
            style = new GUIStyle();
            style.font = RDString.GetFontDataForLanguage(RDString.language).font;
            style.normal.textColor = Color.white;
            style.fontSize = 20;
        }
        public void AddEvent(ADOFAI.LevelEventType type)
        {
            scnEditor editor = scnEditor.instance;
            editor.AddEvent(editor.selectedFloors[0].seqID, type);
        }
        public void Create(float angle) => typeof(scnEditor).GetMethod("CreateFloorWithCharOrAngle", AccessTools.all).Invoke(scnEditor.instance, new object[] { angle, '?', Main.set.pulseFloorButtons, Main.set.fullSpin });
        public bool IsGUI = false;
        readonly Rect rect = new Rect(860, 505, 200, 100);
        readonly Rect slider = new Rect(10, 85, 180, 50);
        public void OnGUI()
        {
            if (!IsGUI) return;
            GUI.Box(rect, "");
            GUI.BeginGroup(rect);
            Main.set.pulseFloorButtons = GUI.Toggle(new Rect(40, 5, 120, 15), Main.set.pulseFloorButtons, "pulseFloorButtons");
            Main.set.fullSpin = GUI.Toggle(new Rect(50, 20, 100, 15), Main.set.fullSpin, "fullSpin");
            /*if (GUI.Button(new Rect(50, 10, 100, 30), "Create!"))
            {
                Create(Main.set.angle);
            }*/
            GUI.Label(new Rect(25, 25, 25, 3), "Min", style);
            float.TryParse(GUI.TextField(new Rect(25, 43, 25, 20), Main.set.min.ToString(), style), out Main.set.min);
            GUI.Label(new Rect(152, 25, 25, 3), "Max", style);
            float.TryParse(GUI.TextField(new Rect(150, 43, 25, 20), Main.set.max.ToString(), style), out Main.set.max);
            if (GUI.Button(new Rect(50, 40, 100, 25), "Circle"))
            {
                if (Main.set.angle != 0)
                {
                    for (float i = Main.set.min; i < Main.set.max; i += Main.set.angle)
                    {
                        Create(i);
                        for (int ii = 0; ii < Main.set.types.Count; ii++)
                        {
                            AddEvent(Main.set.types[ii]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Main.set.max; i++)
                    {
                        Create(Main.set.min);
                        for (int ii = 0; ii < Main.set.types.Count; ii++)
                        {
                            AddEvent(Main.set.types[ii]);
                        }
                    }
                }
            }
            float.TryParse(GUI.TextField(new Rect(75, 65, 50, 20), Main.set.angle.ToString(), style), out Main.set.angle);
            Main.set.angle = GUI.HorizontalSlider(slider, Main.set.angle, 0, 360);
            GUI.EndGroup();
        }
        public void Update()
        {
            if (Main.set.KeyBindSettings.sc.SC.Down && ADOBase.sceneName == "scnEditor" && scrController.instance.isEditingLevel)
            {
                IsGUI = !IsGUI;
            }
            if (Main.set.KeyBindSettings.g.G.Down && ADOBase.sceneName == "scnEditor" && scrController.instance.isEditingLevel)
            {
                Create(Main.set.angle);
                for (int ii = 0; ii < Main.set.types.Count; ii++)
                {
                    AddEvent(Main.set.types[ii]);
                }
            }
        }
    }
}
