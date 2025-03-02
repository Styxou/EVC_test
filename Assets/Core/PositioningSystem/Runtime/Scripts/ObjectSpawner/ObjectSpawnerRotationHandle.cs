using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Athena.PositioningSystem
{
    [AddComponentMenu(Constants.MenuRoot + "Object Spawner Rotation Handle")]
    [DisallowMultipleComponent]
    public class ObjectSpawnerRotationHandle : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        //Paramètres
        [SerializeField, Min(0f)] private float m_RotationSpeed = 1f;
        [SerializeField] private UnityEvent<float> m_OnRotate   = new();
        [SerializeField] private UnityEvent m_OnEndManipulation = new();

        public void OnDrag (PointerEventData pEventData)
        {
            if (pEventData.button == PointerEventData.InputButton.Left)
            {
                m_OnRotate?.Invoke(-pEventData.delta.x * m_RotationSpeed);
            }
        }

        public void OnEndDrag (PointerEventData pEventData)
        {
            m_OnEndManipulation?.Invoke();
        }
    }
}