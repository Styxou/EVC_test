using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialZoneUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialTextUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialTextUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialTextUI.SetActive(false);
        }
    }
}