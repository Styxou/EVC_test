using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BasicEnemy : MonoBehaviour
{
    [Header("References")]
    public GameObject EnemyTarget;
    GameObject EnemyObject;
    NavMeshAgent EnemyAgent;
    public EnemyData data;

    [Header("Stats")]
    private float CurrentAttackCouldown;
    private int currentHealth;


    [Header("Navmesh")]
    private NavMeshPath Path;
    private GameObject Child;

    public event System.Action<BasicEnemy> OnDestroyed;

    private void Start()
    {
        EnemyObject = Instantiate(data.Prefab.gameObject,transform);
        EnemyAgent = EnemyObject.AddComponent<NavMeshAgent>();
        Path = new NavMeshPath();
        Child = transform.GetChild(0).gameObject;

        SetCharacteristics();

    }

    private void Update()
    {
        if(EnemyTarget != null)
        {
            CheckPath();

        }
    }

    private void CheckPath()
    {
        ///Check if the path of the agent is blocked
        EnemyAgent.CalculatePath(EnemyTarget.transform.position, Path);

        if(Path.status == NavMeshPathStatus.PathPartial)
        {
            DetectObstacle();

        }
        else
        {
            EnemyAgent.isStopped = false;
        }
      

    }

    private void DetectObstacle()
    {
        int layerMask = 1 << 7;

        // This would cast rays only against colliders in layer 7.
        RaycastHit hit;
        if (Physics.Raycast(Child.transform.position, Child.transform.TransformDirection(Vector3.forward), out hit, 1, layerMask))
        {
            Wall Obstacle = hit.transform.gameObject.GetComponent<Wall>();
            AttackObstacle(Obstacle);
            Debug.DrawRay(Child.transform.position, Child.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("touche un mur");

        }

    }

    //Attacking Obstacle
    private void AttackObstacle(Wall Obstacle)
    {
        EnemyAgent.isStopped= true;

        if (CurrentAttackCouldown > 0)
        {
            CurrentAttackCouldown--;
        }
        else
        {
            CurrentAttackCouldown = data.AttackCouldown;
            Obstacle.TakeDamage(data.AttackPower);
        }

    }

    private void SetCharacteristics()
    {
        EnemyAgent.speed = data.speed;
        EnemyAgent.angularSpeed = 300;
        EnemyAgent.acceleration = 60;

        currentHealth = data.health;

        GameObject coreObject = GameObject.FindGameObjectWithTag("Core");
        EnemyTarget = coreObject ? coreObject.transform.GetChild(0).gameObject : null;
        if (EnemyTarget)
        {
            EnemyAgent.destination = EnemyTarget.transform.position;
        }
    }

    //Take damage by player
    public void TakeDommage(int damage)
    {
        transform.DOShakeScale(0.2f, strength: new Vector3(0, 1, 0), vibrato: 3, randomness: 0, fadeOut: true);

        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        ///Deal damage to core
        if(other.gameObject.GetComponent<CoreScript>() != null)
        {
            if (CurrentAttackCouldown > 0)
            {
                CurrentAttackCouldown--;
            }
            else
            {
                CurrentAttackCouldown = data.AttackCouldown;
                CoreScript Core = other.gameObject.GetComponent<CoreScript>();
                Core.TakeDommageCore(data.AttackPower);
            }
        }
    }

    protected void OnDestroy ()
    {
        OnDestroyed?.Invoke(this);
    }
}