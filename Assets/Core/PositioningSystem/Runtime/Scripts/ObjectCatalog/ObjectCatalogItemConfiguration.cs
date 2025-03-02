using UnityEngine;

namespace Athena.PositioningSystem
{
    [CreateAssetMenu(menuName = Constants.MenuRoot + "Object Catalog Item Configuration", fileName = "New Object Catalog Item Configuration")]
    public class ObjectCatalogItemConfiguration : ScriptableObject
    {
        //Paramètres
        [SerializeField] private Sprite m_ObjectIcon;
        [SerializeField] private GameObject m_ObjectPrefab;
        [SerializeField] private string m_ObjectName        = string.Empty;
        [SerializeField] private string m_ObjectDescription = string.Empty;

        //Propriétés
        public Sprite ObjectIcon        => m_ObjectIcon;
        public GameObject ObjectPrefab  => m_ObjectPrefab;
        public string ObjectName        => m_ObjectName;
        public string ObjectDescription => m_ObjectDescription;
    }
}