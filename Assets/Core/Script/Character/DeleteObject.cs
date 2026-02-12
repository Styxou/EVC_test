using UnityEditor.UI;
using UnityEngine;



namespace Athena.Prototype
{
    public class OpenDoor : MonoBehaviour
    {
        // Cette fonction se déclenche quand un autre objet entre en collision avec celui-ci
        private void OnCollisionEnter(Collision collision)
        {
            // On vérifie si l'objet qui nous a touché a le tag "Player"
            if (collision.gameObject.CompareTag("Player"))
            {
               
                // Détruit l'objet sur lequel le script est attaché
                Destroy(gameObject);

                Debug.Log("L'objet a été détruit par le joueur !");
            }
        }
    }
}
