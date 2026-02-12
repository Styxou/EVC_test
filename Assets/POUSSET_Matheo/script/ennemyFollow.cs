using UnityEngine;
using System.Collections;

namespace Athena.Prototype
{
    public class CharacterFollow : MonoBehaviour
    {
        public GameObject targetObject;
        public float speed = 3f;
        private bool follow = false;

        void Update()
        {
            if (!follow || targetObject == null) return;

            Vector3 targetPos = targetObject.transform.position;
            targetPos.y = transform.position.y; // garder la hauteur

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == targetObject)
            {
                follow = true;
            }
        }

    }
}