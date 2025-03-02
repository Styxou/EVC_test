using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Athena.PositioningSystem
{
    [AddComponentMenu(Constants.MenuRoot + "Pointer Event Trigger")]
    [DisallowMultipleComponent]
    public class PointerEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        //Paramètres
        [SerializeField] private bool m_CallPointerExitOnEnable = false;
        [SerializeField] private UnityEvent m_OnPointerEnter    = new();
        [SerializeField] private UnityEvent m_OnPointerExit     = new();
        [SerializeField] private UnityEvent m_OnPointerClick    = new();

        public void OnPointerEnter (PointerEventData pEventData)
        {
            m_OnPointerEnter?.Invoke();
        }

        public void OnPointerExit (PointerEventData pEventData)
        {
            m_OnPointerExit?.Invoke();
        }

        public void OnPointerClick (PointerEventData pEventData)
        {
            if (pEventData.button == PointerEventData.InputButton.Left)
            {
                m_OnPointerClick?.Invoke();
            }
        }

        protected void OnEnable ()
        {
            //Initialisation
            if (m_CallPointerExitOnEnable)
            {
                m_OnPointerExit?.Invoke();
            }
        }
    }
}