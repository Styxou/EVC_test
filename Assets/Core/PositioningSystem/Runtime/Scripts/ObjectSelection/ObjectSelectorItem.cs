using UnityEngine;
using UnityEngine.Events;

namespace Athena.PositioningSystem
{
    [AddComponentMenu(Constants.MenuRoot + "Object Selector Item")]
    [DisallowMultipleComponent]
    public class ObjectSelectorItem : MonoBehaviour
    {
        //Paramètres
        [Header("Data")]
        [SerializeField] private ObjectCatalogItemConfiguration m_Item;

        [Header("Update")]
        [SerializeField] private UnityEvent<Sprite> m_OnItemObjectIconUpdated        = new();
        [SerializeField] private UnityEvent<GameObject> m_OnItemObjectPrefabUpdated  = new();
        [SerializeField] private UnityEvent<string> m_OnItemObjectNameUpdated        = new();
        [SerializeField] private UnityEvent<string> m_OnItemObjectDescriptionUpdated = new();

#if UNITY_EDITOR
        protected void OnValidate ()
        {
            UpdateItem();
        }
#endif

        //Propriétés
        public ObjectCatalogItemConfiguration Item
        {
            get { return m_Item; }
            set
            {
                m_Item = value;
                if (isActiveAndEnabled)
                {
                    UpdateItem();
                }
            }
        }

        //Evénements
        public event System.Action<ObjectCatalogItemConfiguration> OnSelect;

        //Mise à jour
        private void UpdateItem ()
        {
            m_OnItemObjectIconUpdated?.Invoke(Item ? Item.ObjectIcon : null);
            m_OnItemObjectPrefabUpdated?.Invoke(Item ? Item.ObjectPrefab : null);
            m_OnItemObjectNameUpdated?.Invoke(Item ? Item.ObjectName : string.Empty);
            m_OnItemObjectDescriptionUpdated?.Invoke(Item ? Item.ObjectDescription : string.Empty);
        }

        //Sélection
        public void Select ()
        {
            OnSelect?.Invoke(Item);
        }

        protected void OnEnable ()
        {
            //Initialisation
            UpdateItem();
        }
    }
}