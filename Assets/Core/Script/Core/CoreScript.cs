using UnityEngine;
using DG.Tweening;

public class CoreScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int health;

    public void TakeDommageCore(int damage)
    {
        health -= damage;

        transform.DOShakeScale(0.2f, strength: new Vector3(0, 1, 0), vibrato: 3, randomness: 0, fadeOut: true);

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}