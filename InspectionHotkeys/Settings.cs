using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace InspectionHotkeys
{

    public class Settings : UnityModManager.ModSettings
    {

        #region Properties
        public static bool Enabled { get; private set; }
        //static doesn't serialize to settings (duh!)
        public bool EnableLogging = false;

        //  if initialized here - deserealization goes down for some reason;
        // couldn't make dictionary serialization work
        public List<ActionKeys> actionList;
        public List<KeyCode> keyList;

        //Dictionary entry serializes fine but is even more unwieldly
        //public List<DictionaryEntry> keylist = new List<DictionaryEntry> {
        //    new DictionaryEntry(ActionKeys.SendPatientHome, KeyCode.R),
        //  new DictionaryEntry(ActionKeys.SendPatientTreatment, KeyCode.T ),
        //   new DictionaryEntry(ActionKeys.FireStaff, KeyCode.F ),
        //   new DictionaryEntry(ActionKeys.EditRoom, KeyCode.E ),
        //};
        #endregion

        #region Methods

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        #endregion
    }
}