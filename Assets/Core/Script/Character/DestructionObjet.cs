using UnityEngine;

namespace Athena.Prototype
{
    public class DestructionObjet : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            // On vérifie si l'objet qui nous a touché a le tag "Player"
            if (collision.gameObject.CompareTag("Player"))
            {
                // On récupère le script InventaireJoueur qui est sur le joueur
                InventaireJoueur inventaire = collision.gameObject.GetComponent<InventaireJoueur>();

                if (inventaire != null)
                {
                    // On passe le booléen à true
                    inventaire.aRamasseObjet = true;
                    Debug.Log("Booléen mis à jour !");
                }

                // On détruit l'objet
                Destroy(gameObject);
            }
        }
    }
}