using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Athena.PositioningSystem
{
    [AddComponentMenu(Constants.MenuRoot + "Object Selector")]
    [DisallowMultipleComponent]
    public class ObjectSelector : MonoBehaviour
    {
        //Paramètres
        [SerializeField] private ObjectCatalogConfiguration m_Catalog;
        [SerializeField] private ObjectSelectorItem m_ItemTemplate;
        [SerializeField] private ObjectSpawner m_TargetSpawner;
        [SerializeField] private UnityEvent m_OnDisplaySelection = new();
        [SerializeField] private UnityEvent m_OnHideSelection    = new();

        //Membres
        private readonly List<ObjectSelectorItem> m_DisplayedItems = new();
        private ObjectPool<ObjectSelectorItem> m_ItemPool;

        //Affichage
        public void DisplaySelection (ObjectSpawner pSpawner)
        {
            m_TargetSpawner = pSpawner;
            DisplaySelection();
        }
        public void DisplaySelection ()
        {
            HideSelection();

            //Pas de cible
            if (!m_TargetSpawner)
            {
                return;
            }

            //Objets
            for (int i = 0; i < m_Catalog.Items.Count; i++)
            {
                if (m_TargetSpawner.AllowObject(m_Catalog.Items[i]))
                {
                    ObjectSelectorItem item = m_ItemPool.Get();
                    item.Item               = m_Catalog.Items[i];
                    m_DisplayedItems.Add(item);
                }
            }

            if (m_DisplayedItems.Count > 0)
            {
                m_OnDisplaySelection?.Invoke();
            }
        }

        //Fermeture
        public void HideSelection ()
        {
            for (int i = 0; i < m_DisplayedItems.Count; i++)
            {
                m_ItemPool.Release(m_DisplayedItems[i]);
            }
            m_DisplayedItems.Clear();
            m_OnHideSelection?.Invoke();
        }

        //Sélection
        private void OnSelectItem (ObjectCatalogItemConfiguration pItem)
        {
            if (m_TargetSpawner)
            {
                m_TargetSpawner.Item = pItem;
            }
            HideSelection();
        }

        //Pool
        private ObjectSelectorItem PoolCreate ()
        {
            ObjectSelectorItem item = Instantiate(m_ItemTemplate, m_ItemTemplate.transform.parent);
            item.OnSelect          += OnSelectItem;
            item.gameObject.SetActive(false);
            return item;
        }
        private void PoolGet (ObjectSelectorItem pItem)
        {
            pItem.gameObject.SetActive(true);
            pItem.transform.SetAsLastSibling();
        }
        private void PoolRelease (ObjectSelectorItem pItem)
        {
            pItem.gameObject.SetActive(false);
        }
        private void PoolDestroy (ObjectSelectorItem pItem)
        {
            Destroy(pItem.gameObject);
        }

        protected void OnEnable ()
        {
            //Initialisation
            HideSelection();
        }

        protected void OnDisable ()
        {
            //Réinitialisation
            HideSelection();
        }

        protected void Awake ()
        {
            //Initialisation
            m_ItemPool = new ObjectPool<ObjectSelectorItem>(PoolCreate, PoolGet, PoolRelease, PoolDestroy);
            m_ItemTemplate.gameObject.SetActive(false);
        }

        protected void OnDestroy ()
        {
            //Réinitialisation
            m_ItemPool?.Dispose();
        }
    }
}