using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Harmony12;
using UnityEngine;
using UnityModManagerNet;
using UnityEditor;

//[assembly: AssemblyVersion("0.45.*")]
namespace InspectionHotkeys
{
    
    public enum ActionKeys
    {
        None,
        SendPatientHome,
        SendPatientTreatment,
        PromoteStaff,
        FireStaff,
        EditRoomItems,
        EditRoom,
        CopyRoom,
        SellRoom
    }

    [EnableReloading]
    internal static class Program
    {
        #region Properties
        public static ActionKeys WaitingForKey = ActionKeys.None;
        public static bool Enabled { get; private set; }
        public static bool IsVisibleUI = false;
        public static Settings Settings { get; private set; }
        public static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        private static Dictionary<ActionKeys, System.Action> funcDict = new Dictionary<ActionKeys, System.Action>
                {
                    {ActionKeys.SendPatientHome, Patches.InspectorDataPatient_SelectPatient_Patch.SendHomeIfPossible},
                    {ActionKeys.SendPatientTreatment, Patches.InspectorDataPatient_SelectPatient_Patch.SendForTreatmentIfPossible},
                    {ActionKeys.FireStaff, Patches.InspectorDataStaff_SelectStaff_Patch.FireIfPossible},
                    {ActionKeys.PromoteStaff, Patches.InspectorDataStaff_SelectStaff_Patch.PromoteIfPossible},
                    {ActionKeys.EditRoomItems, Patches.InspectorDataRoom_SelectRoom_Patch.EditRoomItems},
                    {ActionKeys.EditRoom, Patches.InspectorDataRoom_SelectRoom_Patch.EditRoom},
                    {ActionKeys.CopyRoom, Patches.InspectorDataRoom_SelectRoom_Patch.CopyRoom},
                    {ActionKeys.SellRoom, Patches.InspectorDataRoom_SelectRoom_Patch.SellRoom}
                };


        #endregion

        #region Methods

        private static bool Load(UnityModManager.ModEntry modEntry)
        {


            HarmonyInstance.Create(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());

            Settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Logger = modEntry.Logger;

            modEntry.OnToggle = OnToggle;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnShowGUI = OnShowGUI;
            modEntry.OnHideGUI = OnHideGUI;
            modEntry.OnUpdate = OnUpdate;
            return true;
        }

        public static void InvokeMethod(System.Type type, object _instance, string methodName, object[] args = null)
        {
            try
            {
                System.Reflection.MethodInfo methodInfo;
                methodInfo = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (methodInfo == null)
                    Error($"Didn't find the method {methodName} in instance {_instance} of {type}");
                methodInfo.Invoke(_instance, args);
            }
            catch (System.Exception e)
            {
                Error(e.ToString());
            }
        }

        #endregion

        #region Event handler

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        public static void Log(string msg)
        {
            if (Settings.EnableLogging)
                Logger.Log(msg);
        }

        public static void Error(string msg)
        {
            //if (Settings.EnableLogging)
            Logger.Error(msg);
        }

        private static void OnShowGUI(UnityModManager.ModEntry modEntry)
        {
            IsVisibleUI = true;
        }

        private static void OnHideGUI(UnityModManager.ModEntry modEntry)
        {
            IsVisibleUI = false;
        }

        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Settings.actionList == null)
            {
                Dictionary<ActionKeys, KeyCode> _keydict = new Dictionary<ActionKeys, KeyCode>
                {
                    {ActionKeys.SendPatientHome, KeyCode.R },
                    {ActionKeys.SendPatientTreatment, KeyCode.T},
                    {ActionKeys.PromoteStaff, KeyCode.Y},
                    {ActionKeys.FireStaff, KeyCode.F},
                    {ActionKeys.EditRoomItems, KeyCode.I},
                    {ActionKeys.EditRoom, KeyCode.E},
                    {ActionKeys.CopyRoom, KeyCode.C},
                    {ActionKeys.SellRoom, KeyCode.S}
                };
                Settings.actionList = _keydict.Keys.ToList<ActionKeys>();
                Settings.keyList = _keydict.Values.ToList<KeyCode>();
                _keydict.Clear();
            }

            Dictionary<ActionKeys, KeyCode> keydict = Settings.actionList.Zip(Settings.keyList, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            foreach (KeyValuePair<ActionKeys, KeyCode> entry in keydict)
            {
                GUILayout.BeginHorizontal();
                //GUI.skin.label.CalcSize()
                GUILayout.Label($"Key for action {entry.Key.ToString()}", GUILayout.Width(240.0f));
                if (GUILayout.Button(entry.Value.ToString(), GUILayout.Width(120.0f)))
                {
                    WaitingForKey = entry.Key;
                }
                GUILayout.EndHorizontal();
            }

            keydict.Clear();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Enable logging ", GUILayout.ExpandWidth(false));
            Settings.EnableLogging = (GUILayout.Toggle((Settings.EnableLogging ? 1 : 0) != 0, "", GUILayout.ExpandWidth(false)) ? 1 : 0) != 0;
            GUILayout.EndHorizontal();
        }

        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            Settings.Save(modEntry);
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            //
            //detect key
            if (WaitingForKey != ActionKeys.None)
            {
                foreach (KeyCode keySearch in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keySearch))
                    {
                        if (!keySearch.ToString().StartsWith("Mouse"))
                        {
                            KeyCode setKey = keySearch;
                            //clear key
                            if (setKey == KeyCode.Backspace)
                                setKey = KeyCode.None;
                            try
                            {
                                Log("Key waiting for = " + keySearch.ToString());
                                Dictionary<ActionKeys, KeyCode> keydict = Settings.actionList.Zip(Settings.keyList, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
                                keydict[WaitingForKey] = setKey;
                                Settings.actionList.Clear();
                                Settings.keyList.Clear();
                                Settings.actionList = keydict.Keys.ToList<ActionKeys>();
                                Settings.keyList = keydict.Values.ToList<KeyCode>();
                                keydict.Clear();
                            }
                            catch (System.Exception e)
                            {

                                Error(e.ToString());
                            }
                            WaitingForKey = ActionKeys.None;
                        }
                    }
                }
            }
            else
            //keyReaction
            {
                if (!IsVisibleUI)
                {
                    Dictionary<ActionKeys, KeyCode> keydict = Settings.actionList.Zip(Settings.keyList, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

                    foreach (KeyValuePair<ActionKeys, KeyCode> entry in keydict)
                    {
                        if (Input.GetKeyDown(entry.Value))
                        {
                            funcDict[entry.Key]();
                        }
                    }

                    keydict.Clear();
                }
            }
        }

        #endregion
    }

}