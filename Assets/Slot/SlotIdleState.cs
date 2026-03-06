using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

namespace Slot
{
    [State("Idle")]
    public class SlotIdleState : FSMState
    {
        [Enter]
        private void EnterThis()
        {
            Settings.Model.Set("BtnStartEnable", true);
            Settings.Model.Set("BtnStopEnable", false);
        }

        [Bind("DoStart")]
        private void OnDoStart()
        {
            Parent.Change("Rolling");
            Settings.Model.EventManager.Invoke("SlotStartScroll");
        }

        [Exit]
        private void ExitThis() { }
    }
}
