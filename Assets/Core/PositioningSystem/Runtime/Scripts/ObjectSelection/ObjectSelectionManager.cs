namespace Athena.PositioningSystem
{
    public static class ObjectSelectionManager
    {
        //Evénements
        public static event System.Action<ObjectSpawner> OnSelectSpawner;

        //Sélection d'un point de spawn
        public static void SelectSpawner (ObjectSpawner pSpawner)
        {
            OnSelectSpawner?.Invoke(pSpawner);
        }
    }
}