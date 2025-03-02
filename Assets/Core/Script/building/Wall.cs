using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Wall : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] int Health;
    private int currentHealth;
    private void Start()
    {
        currentHealth = Health;

    }

    ///Take Damage by enemy
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        transform.DOShakeScale(0.2f, strength: new Vector3(0, 1, 0), vibrato: 3, randomness: 0, fadeOut: true);


        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

     

}

