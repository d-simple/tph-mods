using System;
using Harmony12;
using TH20;

using BindingFlags = System.Reflection.BindingFlags;

namespace InspectionHotkeys.Patches
{
    [HarmonyPatch(typeof(InspectorDataPatient), "SelectPatient")]

    public class InspectorDataPatient_SelectPatient_Patch
    {
        public enum PatientInspectButton
        {
            Vaccinate = 1,
            SendForTreatment = 2,
            SendHome = 3
        }

        #region Properties

        private static InspectorDataPatient _instance;
        public static HUD _HUD { get; private set; }

        #endregion

        #region Methods

        private static void Prefix(InspectorDataPatient __instance, Patient patient, Level ___Level)
        {
            if (!Program.Enabled)
                return;

            Program.Log(patient.Name);

            if (__instance != null)
            {
                _instance = __instance;
                Program.Log("Instance set");
            }

            Program.Log("Instance = " + _instance);

            _HUD = ___Level.HUD;
            //_InspectorMenu = _HUD.FindMenu<InspectorMenu>(false);

            //can't access private methods by triple underlines argument - patching breaks
            //SendHome = ___SendHome;
            //SendForTreatment = ___SendForTreatment;

            //cant call public method on argument __instance for some reason 
            //inspector stops working ingame. What the?
            //canSendHome = __instance.IsFooterButtonVisible(i);
        }

        public static void SendHomeIfPossible()
        {
            if (_instance != null)
                try
                {
                    int i;
                    i = (int)PatientInspectButton.SendHome;
                    bool canSendHome = _instance.IsFooterButtonVisible(i) && _instance.IsFooterButtonEnabled(i);

                    InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                    //Don't do stuff if inspect is hidden
                    if (imenu != null & canSendHome)
                    {
                        //send home. can't call private method directly
                        Program.InvokeMethod(typeof(InspectorDataPatient), _instance, "SendHome");
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e.ToString());
                }
        }

        public static void SendForTreatmentIfPossible()
        {
            if (_instance != null)
                try
                {
                    int i;
                    i = (int)PatientInspectButton.SendForTreatment;
                    bool canTreat = _instance.IsFooterButtonVisible(i) && _instance.IsFooterButtonEnabled(i);

                    InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                    //Don't do stuff if inspect is hidden
                    if (imenu != null & canTreat)
                    {
                        //treat. can't call private method directly
                        Program.InvokeMethod(typeof(InspectorDataPatient), _instance, "SendForTreatment");
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e.ToString());
                }
        }

        #endregion
    }
}