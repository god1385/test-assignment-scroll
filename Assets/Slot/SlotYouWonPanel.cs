using AxGrid.Base;
using AxGrid.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Slot
{
    public class SlotYouWonPanel : MonoBehaviourExtBind
    {
        [SerializeField] private SlotView slotView;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Image iconImage;
        [SerializeField] private float showDuration = 3f;

        [OnStart]
        private void initialize() => panelRoot.SetActive(false);

        [Bind("SlotStopped")]
        private void OnSlotStopped()
        {
            Sprite s = slotView.GetCenterCellSprite();
            iconImage.sprite = s;

            panelRoot.SetActive(true);
            CancelInvoke(nameof(Hide));
            Invoke(nameof(Hide), showDuration);
        }

        private void Hide() => panelRoot.SetActive(false);
    }
}
