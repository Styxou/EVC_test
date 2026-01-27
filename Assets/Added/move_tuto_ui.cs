using UnityEngine;

public class ZoneTexte : MonoBehaviour
{
    public GameObject texteUI;

    private void Start()
    {
        texteUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            texteUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            texteUI.SetActive(false);
        }
    }
}