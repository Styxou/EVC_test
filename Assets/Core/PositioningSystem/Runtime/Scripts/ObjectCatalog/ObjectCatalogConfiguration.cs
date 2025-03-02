using System.Collections.Generic;
using UnityEngine;

namespace Athena.PositioningSystem
{
    [CreateAssetMenu(menuName = Constants.MenuRoot + "Object Catalog Configuration", fileName = "New Object Catalog Configuration")]
    public class ObjectCatalogConfiguration : ScriptableObject
    {
        //Paramètres
        [SerializeField] private List<ObjectCatalogItemConfiguration> m_Items = new();

        //Propriétés
        public IReadOnlyList<ObjectCatalogItemConfiguration> Items => m_Items;
    }
}