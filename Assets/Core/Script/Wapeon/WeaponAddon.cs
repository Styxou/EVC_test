using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAddon : MonoBehaviour
{
    [Header("Mods")]
    [SerializeField ]WeaponChained ModChained;

    [Header("State")]
    public int damage;
    public bool Chained;

    private MeshRenderer Mesh;

    [Header("Timer")]
    public float lifetimeTimer;
    private float lifetime;

    [Header("Vfx")]
    [SerializeField] GameObject pafVfx;

    // Start is called before the first frame update
    void Start()
    {
        Mesh = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        LifeTime();
    }

    public void LifeTime()
    {
        if (lifetimeTimer > 0)
        {
            lifetimeTimer -= Time.deltaTime;
        }

        if (lifetimeTimer < 0)
        {
            Destroy(gameObject);
            ResetTimer();
        }

    }

    private void ResetTimer()
    {
        lifetimeTimer = lifetime;
    }

    private void OnCollisionEnter(Collision collision)
    {       
        ///collision with an enemy
        if (collision.gameObject.GetComponent<BasicEnemy>() != null)
        {

            BasicEnemy enemy = collision.gameObject.GetComponent<BasicEnemy>();
            Vector3 hitPosition = collision.contacts[0].point;

            enemy.TakeDommage(damage);

            ///Instantiate VFX hit enemy
            Instantiate(pafVfx, hitPosition, Quaternion.identity);

            ///mod chained
            if(Chained == true)
            {

                ModChained.ChainedDamage(enemy.gameObject);
                Mesh.enabled = false;

            }
            else if (Chained == false)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
