using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Athena.PositioningSystem
{
    [AddComponentMenu(Constants.MenuRoot + "Object Spawner")]
    [DisallowMultipleComponent]
    public class ObjectSpawner : MonoBehaviour
    {
        //Paramètres
        [Header("Parameters")]
        [SerializeField] private ObjectCatalogItemConfiguration m_Item;
        [SerializeField] private Transform m_CustomSpawnPoint;
        [SerializeField] private bool m_AllowRotation                                = true;
        [SerializeField] private List<ObjectCatalogItemConfiguration> m_AllowedItems = new();

        [Header("Events")]
        [SerializeField] private UnityEvent<ObjectSpawner> m_OnInteract  = new();
        [SerializeField] private UnityEvent<bool> m_OnShowRotationHandle = new();

        //Propriétés
        public ObjectCatalogItemConfiguration Item
        {
            get { return m_Item; }
            set
            {
                if (m_Item != value)
                {
                    m_Item = value;
                    SpawnObject();
                }
            }
        }
        public bool HasObject => m_SpawnedObject;

        //Membres
        private GameObject m_SpawnedObject;

#if UNITY_EDITOR
        [ContextMenu("Spawn Object", true)]
        private bool CanSpawnObject ()
        {
            return UnityEditor.EditorApplication.isPlaying && Item;
        }
#endif

        //Possibilité de placer un objet
        public bool AllowObject (ObjectCatalogItemConfiguration pItem)
        {
            return pItem && (m_AllowedItems.Count == 0 || m_AllowedItems.Contains(pItem));
        }

        //Mise à jour de l'objet
        [ContextMenu("Spawn Object")]
        private void SpawnObject ()
        {
            //Destruction de l'objet actuel
            if (HasObject)
            {
                Destroy(m_SpawnedObject);
            }

            //Ajout du nouvel objet
            if (Item && Item.ObjectPrefab)
            {
                Transform root  = m_CustomSpawnPoint ? m_CustomSpawnPoint : transform;
                m_SpawnedObject = Instantiate(Item.ObjectPrefab, root.position, root.rotation, root);

                SetRotationHandleVisible(m_AllowRotation);
            }
            else
            {
                SetRotationHandleVisible(false);
            }
        }

        //Interaction
        public void Interact ()
        {
            m_OnInteract?.Invoke(this);
        }

        //Visibilité des poignées d'orientation
        public void SetRotationHandleVisible (bool pIsVisible)
        {
            m_OnShowRotationHandle?.Invoke(pIsVisible);
        }

        //Changement d'orientation
        public void RotateObject (float pAngle)
        {
            if (HasObject)
            {
                m_SpawnedObject.transform.localRotation *= Quaternion.Euler(0f, pAngle, 0f);
            }
        }

        protected void Awake ()
        {
            //Initialisation
            m_AllowedItems.RemoveAll(x => !x);
            SpawnObject();
        }
    }
}