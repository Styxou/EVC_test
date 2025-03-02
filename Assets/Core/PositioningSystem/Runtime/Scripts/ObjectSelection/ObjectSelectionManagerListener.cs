using UnityEngine;
using UnityEngine.Events;

namespace Athena.PositioningSystem
{
    [AddComponentMenu(Constants.MenuRoot + "Object Selection Manager Listener")]
    [DisallowMultipleComponent]
    public class ObjectSelectionManagerListener : MonoBehaviour
    {
        //Paramètres
        [SerializeField] private UnityEvent<ObjectSpawner> m_OnSelectSpawner = new();

        //Sélection d'un point de spawn
        private void OnSelectSpawnerListener (ObjectSpawner pSpawner)
        {
            m_OnSelectSpawner?.Invoke(pSpawner);
        }

        //Sélection d'un point de spawn
        public void NotifySpawnerSelected (ObjectSpawner pSpawner)
        {
            ObjectSelectionManager.SelectSpawner(pSpawner);
        }

        protected void OnEnable ()
        {
            //Initialisation
            ObjectSelectionManager.OnSelectSpawner += OnSelectSpawnerListener;
        }

        protected void OnDisable ()
        {
            //Réinitialisation
            ObjectSelectionManager.OnSelectSpawner -= OnSelectSpawnerListener;
        }
    }
}