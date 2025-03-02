using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Athena.PositioningSystem
{
    [CustomEditor(typeof(ObjectCatalogItemConfiguration), true), CanEditMultipleObjects]
    public class ObjectCatalogItemConfigurationEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            DrawDefaultInspector();

            foreach (ObjectCatalogItemConfiguration item in targets.OfType<ObjectCatalogItemConfiguration>().OrderBy(x => x.name))
            {
                EditorGUILayout.Space();
                DrawItemPreview(item);
            }
        }

        //Aperçu
        public static void DrawItemPreview (ObjectCatalogItemConfiguration pItem)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                //Nom/Description
                EditorGUILayout.LabelField($"{pItem.ObjectName} ({pItem.name})", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(pItem.ObjectDescription);

                //Aperçus
                Rect rect               = EditorGUILayout.GetControlRect(false, 150f);
                Rect iconRect           = new(rect.x, rect.y, rect.width / 2f, rect.height);
                Rect prefabRect         = new(iconRect.xMax, rect.y, iconRect.width, rect.height);
                Texture2D iconPreview   = AssetPreview.GetAssetPreview(pItem.ObjectIcon);
                Texture2D prefabPreview = AssetPreview.GetAssetPreview(pItem.ObjectPrefab);

                if (iconPreview)
                {
                    EditorGUI.DrawTextureTransparent(iconRect, iconPreview, ScaleMode.ScaleToFit);
                }
                if (prefabPreview)
                {
                    EditorGUI.DrawTextureTransparent(prefabRect, prefabPreview, ScaleMode.ScaleToFit);
                }
            }
        }
    }
}