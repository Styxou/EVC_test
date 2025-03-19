using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private GameObject _trap;
    [SerializeField] private GameObject _module;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
