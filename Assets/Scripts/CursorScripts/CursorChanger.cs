using UnityEngine;
using UnityEngine.EventSystems;

namespace SkellyCursor
{
    public class CursorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ModeOfCursor modeOfCursor;

        public void OnPointerEnter(PointerEventData eventData)
        {
            CursorControler.Instance.SetToMode(modeOfCursor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CursorControler.Instance.SetToMode(ModeOfCursor.Default);
        }
    }
}