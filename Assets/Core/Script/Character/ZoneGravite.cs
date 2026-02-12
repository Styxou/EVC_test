using UnityEngine;

namespace Athena.Prototype
{
    public class ZoneGravite : MonoBehaviour
    {
        public float graviteReduite = -2f; // Gravité faible
        public float graviteNormale = -9.81f; // Gravité terrestre standard

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // On vérifie si le joueur possède le script d'inventaire
                InventaireJoueur inventaire = other.GetComponent<InventaireJoueur>();

                // Si le joueur a ramassé l'objet (booléen à true)
                if (inventaire != null && inventaire.aRamasseObjet)
                {
                    // On modifie la gravité globale (ou celle du Rigidbody)
                    Physics.gravity = new Vector3(0, graviteReduite, 0);
                    Debug.Log("Gravité réduite activée !");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Quand le joueur sort de la zone, on remet la gravité par défaut
                Physics.gravity = new Vector3(0, graviteNormale, 0);
                Debug.Log("Retour à la gravité normale.");
            }
        }
    }
}