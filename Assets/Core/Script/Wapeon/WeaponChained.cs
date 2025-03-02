using UnityEngine;
using System.Collections;

public class WeaponChained : MonoBehaviour
{
    [Header("references")]
    [SerializeField] LineRenderer Ln;
    [Header("Characteristics")]
    public float DetectionRadius = 10f;
    public int ChainedDamages = 20;
    public int ChainLenght = 2;
    [SerializeField] float TimerFeedback;
    private float CurrentTimerFeedback;


    bool EnemyTouched = false;
    LayerMask LayerMask;

    private void Start()
    {
        LayerMask = LayerMask.GetMask("Enemy");
        CurrentTimerFeedback = TimerFeedback;
    }

    private void Update()
    {
        if(EnemyTouched == true)
        {
            Timer();
        }
    }

    public void ChainedDamage (GameObject FirstHitEnemy)
    {
        EnemyTouched = true;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRadius, LayerMask);

        GetComponent<Collider>().enabled = false;

        Ln.enabled = true;

        if(hitColliders.Length > ChainLenght)
        {
            Ln.positionCount = ChainLenght;
        }
        else
        {
            Ln.positionCount = hitColliders.Length;
        }

        if (hitColliders.Length > 2)
        {
            for (int i = 0; i < ChainLenght; i++)
            {
                Collider NearEnemy = hitColliders[i];

                Ln.SetPosition(i, NearEnemy.gameObject.transform.position);

                if (NearEnemy != null)
                {
                    BasicEnemy NearEnemyScript = NearEnemy.gameObject.GetComponentInParent<BasicEnemy>();
                    NearEnemyScript.TakeDommage(ChainedDamages);
                }
            }
        }

    }
    private void Timer()
    {
        if (CurrentTimerFeedback > 0)
        {
            CurrentTimerFeedback -= Time.deltaTime;
        }

        if (CurrentTimerFeedback < 0)
        {
            CurrentTimerFeedback = TimerFeedback;
            Destroy(gameObject);
        }
    }


}
