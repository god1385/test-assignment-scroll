using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

namespace Slot
{
    [State("Rolling")]
    public class SlotRollingState : FSMState
    {
        [Enter]
        private void EnterThis()
        {
            Settings.Model.Set("BtnStartEnable", false);
            Settings.Model.Set("BtnStopEnable", false);
        }

        [One(3f)]
        private void UnlockStopButton() => Settings.Model.Set("BtnStopEnable", true);

        [Bind("DoStop")]
        private void OnDoStop()
        {
            Parent.Change("Stopping");
            Settings.Model.EventManager.Invoke("SlotRequestStop");
        }

        [Exit]
        private void ExitThis() { }
    }
}
