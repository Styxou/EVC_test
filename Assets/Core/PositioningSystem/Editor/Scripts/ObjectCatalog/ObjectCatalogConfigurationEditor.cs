using System.Linq;
using UnityEditor;

namespace Athena.PositioningSystem
{
    [CustomEditor(typeof(ObjectCatalogConfiguration), true), CanEditMultipleObjects]
    public class ObjectCatalogConfigurationEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector();

            foreach (ObjectCatalogConfiguration catalog in targets.OfType<ObjectCatalogConfiguration>())
            {
                EditorGUILayout.Space();
                DrawCatalogPreview(catalog);
            }
        }

        //Aperçu
        public static void DrawCatalogPreview (ObjectCatalogConfiguration pCatalog)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(pCatalog.name, EditorStyles.boldLabel);
            }

            foreach (ObjectCatalogItemConfiguration item in pCatalog.Items.Where(x => x))
            {
                ObjectCatalogItemConfigurationEditor.DrawItemPreview(item);
            }
        }
    }
}