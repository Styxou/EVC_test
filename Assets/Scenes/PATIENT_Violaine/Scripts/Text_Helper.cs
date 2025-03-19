using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Text_Helper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI  _textToDisplay;
    [SerializeField] private string  _text;
    [SerializeField] private GameObject _actualLight;
    [SerializeField] private GameObject _nextLight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _textToDisplay.text = "" + _text;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Destroy(_actualLight);
        _nextLight.SetActive(true);
        _textToDisplay.text = "";
        Destroy(gameObject);
    }
}

