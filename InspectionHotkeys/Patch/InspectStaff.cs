using System;
using Harmony12;
using TH20;

using BindingFlags = System.Reflection.BindingFlags;

namespace InspectionHotkeys.Patches
{
    public enum StaffInspectButton
    {
        Pickup,
        Fire,
        Promote,
        Train,
        Vaccinate = 5,
        Breakdown,
    }

    [HarmonyPatch(typeof(InspectorDataStaff), "SelectStaff")]

    public class InspectorDataStaff_SelectStaff_Patch
    {
        #region Properties

        private static InspectorDataStaff _instance;

        public static HUD _HUD { get; private set; }
        public static Staff _staff;

        #endregion

        #region Methods

        private static void Prefix(InspectorDataStaff __instance, Staff staff, Level ___Level)
        {
            if (!Program.Enabled)
                return;

            Program.Log(staff.Name);
            //Program.Log(staff.RankDefinition.FurtherDiagnosisChoiceCount.ToString());

            if (staff != null)
                _staff = staff;

            if (__instance != null)
            {
                _instance = __instance;
                Program.Log("Instance set");
            }

            Program.Log("Instance = " + _instance);

            _HUD = ___Level.HUD;
        }

        public static void PromoteIfPossible()
        {
            if (_instance != null)
                try
                {
                    int i;
                    i = (int)StaffInspectButton.Promote;
                    bool canPromote = _instance.IsFooterButtonVisible(i) && _instance.IsFooterButtonEnabled(i);

                    InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                    //Don't do stuff if inspect is hidden
                    if (imenu != null & canPromote)
                    {
                        _staff.AutoPromote();
                        //treat. can't call private method directly
                        //Program.InvokeMethod(typeof(InspectorDataStaff), _instance, "SendForTreatment");
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e.ToString());
                }
        }

        public static void FireIfPossible()
        {
            if (_instance != null)
                try
                {

                    NotificationMessageUI menu = _HUD.FindMenu<NotificationMessageUI>(false);
                    Program.Log("Menu = " + menu);
                    if (menu != null)
                    {
                        //confirm fire
                        // 0 = Button Ok
                        Program.InvokeMethod(typeof(NotificationMessageUI), menu, "CloseMessage", new object[] { 0 });
                    }
                    else
                    {
                        int i;
                        i = (int)StaffInspectButton.Fire;
                        bool canFire = _instance.IsFooterButtonVisible(i) && _instance.IsFooterButtonEnabled(i);

                        InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                        //Don't do stuff if inspect is hidden
                        if (imenu != null & canFire)
                        {
                            //treat. can't call private method directly
                            Program.InvokeMethod(typeof(InspectorDataStaff), _instance, "Fire");
                        }
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