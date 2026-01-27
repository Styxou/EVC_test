using UnityEngine;

namespace Athena.Prototype
{
    public class CharacterFollow : MonoBehaviour
    {
        public void OnTriggerEnter(Collider collision)
        {
            if (collision.CompareTag("Player"))
            {
                print("je te suis");
            }
        }
        public void OnTriggerExit(Collider collision)
        {
            if(collision.CompareTag("Player"))
            {
                print("je te suis plus");
            }
        }
    }
}
