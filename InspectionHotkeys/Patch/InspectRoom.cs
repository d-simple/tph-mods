using System;
using Harmony12;
using TH20;

using BindingFlags = System.Reflection.BindingFlags;

namespace InspectionHotkeys.Patches
{
    [HarmonyPatch(typeof(InspectorDataRoom), "SelectRoom")]
    public class InspectorDataRoom_SelectRoom_Patch
    {
        #region Properties

        private static InspectorDataRoom _instance;
        private static Room _room;
        public static HUD _HUD { get; private set; }

        #endregion

        #region Methods

        private static void Prefix(InspectorDataRoom __instance, Room room, Level ___Level)
        {
            if (!Program.Enabled)
                return;

            _room = room;
            _HUD = ___Level.HUD;

            Program.Log("Selected:" + room.GetRoomName());

            if (__instance != null)
            {
                _instance = __instance;
                Program.Log("Instance set");
            }

            Program.Log("Instance = " + _instance);

        }

        public static void EditRoomItems()
        {
            if (_instance != null)
                try
                {
                    InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                    //Don't do stuff if inspect is hidden
                    if (imenu != null & InspectorMenu.ShouldShowInspector(_room))
                    {
                        //edit room items
                        Program.InvokeMethod(typeof(InspectorDataRoom), _instance, "EditRoomObjects");
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e.ToString());
                }
        }

        public static void EditRoom()
        {
            if (_instance != null)
                try
                {
                    InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                    //Don't do stuff if inspect is hidden
                    if (imenu != null & InspectorMenu.ShouldShowInspector(_room)) { 
                        //edit room
                        Program.InvokeMethod(typeof(InspectorDataRoom), _instance, "EditRoom");
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e.ToString());
                }
        }

        public static void CopyRoom()
        {
            if (_instance != null)
                try
                {
                    InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                    if (InspectorMenu.ShouldShowInspector(_room))
                    {
                        //copy room 
                        Program.InvokeMethod(typeof(InspectorDataRoom), _instance, "CopyRoom");
                    }
                }
                catch (Exception e)
                {
                    Program.Error(e.ToString());
                }
        }

        public static void SellRoom()
        {
            SelectMenuRoomItem smenu = _HUD.FindMenu<SelectMenuRoomItem>(false);
            if (smenu != null)
            {
                //sell item
                Program.InvokeMethod(typeof(SelectMenuRoomItem), smenu, "SellButton");
                return;

            }

            if (_instance != null)
                try
                {

                    NotificationMessageUI menu = _HUD.FindMenu<NotificationMessageUI>(false);
                    Program.Log("Menu = " + menu);
                    if (menu != null)
                    {
                        //confirm sell
                        // 0 = Button Ok
                        Program.InvokeMethod(typeof(NotificationMessageUI), menu, "CloseMessage", new object[] { 0 });
                    }
                    else
                    {
                        InspectorMenu imenu = _HUD.FindMenu<InspectorMenu>(false);
                        //Don't do stuff if inspect is hidden
                        if (imenu != null & InspectorMenu.ShouldShowInspector(_room))
                        {
                            //show sell dialog
                            Program.InvokeMethod(typeof(InspectorDataRoom), _instance, "SellRoom");
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