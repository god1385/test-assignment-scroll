using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;
using AxGrid.Model;
using UnityEngine;

namespace Slot
{
    public class SlotMain : MonoBehaviourExtBind
    {
        [OnEnable]
        private void OnEnableThis() { }

        [OnStart]
        private void StartThis()
        {
            Settings.Fsm = new FSM();
            Settings.Fsm.Add(new SlotIdleState());
            Settings.Fsm.Add(new SlotRollingState());
            Settings.Fsm.Add(new SlotStoppingState());
            Settings.Fsm.Start("Idle");
        }

        [OnUpdate]
        private void UpdateThis() => Settings.Fsm.Update(Time.deltaTime);

        [Bind("OnStartClick")]
        private void OnStartClicked() => Settings.Fsm.Invoke("DoStart");

        [Bind("OnStopClick")]
        private void OnStopClicked() => Settings.Fsm.Invoke("DoStop");
    }
}
