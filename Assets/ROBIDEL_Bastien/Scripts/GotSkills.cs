using UnityEngine;

namespace Athena.Prototype
{
    public class GotSkills : MonoBehaviour
    {
        public SkillCheckpoint skillCheckpoint;

        public void OnTriggerEnter(Collider collision)
        {
            
            if (collision.CompareTag("Player"))
            {
                skillCheckpoint.CanGrab = true;

                Destroy(gameObject);

            }
        }
    }
}
