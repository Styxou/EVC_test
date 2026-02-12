using UnityEngine;

namespace Athena.Prototype
{
    public class GotSkills : MonoBehaviour
    {
        

        public void OnTriggerEnter(Collider collision)
        {

            if (collision.CompareTag("Player"))
            {
             

                Destroy(gameObject);

            }
        }
    }
}