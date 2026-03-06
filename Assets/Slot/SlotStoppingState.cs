using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

namespace Slot
{
    [State("Stopping")]
    public class SlotStoppingState : FSMState
    {
        [Enter]
        private void EnterThis()
        {
            Settings.Model.Set("BtnStartEnable", false);
            Settings.Model.Set("BtnStopEnable", false);
        }

        [Bind("SlotStopped")]
        private void OnSlotStopped() => Parent.Change("Idle");

        [Exit]
        private void ExitThis() { }
    }
}
