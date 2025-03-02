using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Données de vague d'ennemis
    [System.Serializable]
    private class SpawnerWaveData
    {
        //Paramètres
        [SerializeField] private BasicEnemy[] m_WaveEnemiesPrefabs   = new BasicEnemy[0];
        [SerializeField, Min(0f)] private float m_TimeBeforeWave     = 0f;
        [SerializeField] private bool m_RandomEnemiesOrder           = false;
        [SerializeField, Min(0f)] private float m_TimeBetweenEnemies = 0f;

        //Propriétés
        public BasicEnemy[] WaveEnemiesPrefabs => m_WaveEnemiesPrefabs;
        public float TimeBeforeWave            => m_TimeBeforeWave;
        public bool RandomEnemiesOrder         => m_RandomEnemiesOrder;
        public float TimeBetweenEnemies        => m_TimeBetweenEnemies;
    }

    //Paramètres
    [SerializeField] private SpawnerWaveData[] m_Waves    = new SpawnerWaveData[0];
    [SerializeField, Min(0f)] private float m_SpawnRadius = 0f;
#if UNITY_EDITOR
    [SerializeField] private KeyCode m_DebugSkipWaveKey = KeyCode.End;
#endif

    //Membres
    private readonly List<BasicEnemy> m_SpawnedEnemies = new();

    //Vagues d'ennemis
    private IEnumerator SpawnWaves ()
    {
        int waveIndex = 0;
        while (waveIndex < m_Waves.Length)
        {
#if UNITY_EDITOR
            Debug.Log($"Start wave {waveIndex}", this);
#endif
            SpawnerWaveData wave = m_Waves[waveIndex++];
            if (wave.TimeBeforeWave > 0f)
            {
                yield return new WaitForSeconds(wave.TimeBeforeWave);
            }
            yield return SpawnWaveEnemiesAndWait(wave);
        }
#if UNITY_EDITOR
        Debug.Log("Wave end", this);
#endif
    }

    //Vague d'ennemis
    private IEnumerator SpawnWaveEnemiesAndWait (SpawnerWaveData pWave)
    {
        List<BasicEnemy> waveEnemiesPrefabs = new(pWave.WaveEnemiesPrefabs);
        bool isFirstEnemy                   = true;

        //Spawn
        while (waveEnemiesPrefabs.Count > 0)
        {
            if (!isFirstEnemy && pWave.TimeBetweenEnemies > 0f)
            {
                yield return new WaitForSeconds(pWave.TimeBetweenEnemies);
            }

            int index              = pWave.RandomEnemiesOrder ? Random.Range(0, waveEnemiesPrefabs.Count) : 0;
            Vector2 randomInCircle = Random.insideUnitCircle;
            Vector3 enemyPosition  = transform.position + m_SpawnRadius * new Vector3(randomInCircle.x, 0f, randomInCircle.y);
            BasicEnemy enemy       = Instantiate(waveEnemiesPrefabs[index], enemyPosition, transform.rotation);
            enemy.OnDestroyed     += OnEnemyDestroyed;
            isFirstEnemy           = false;

            waveEnemiesPrefabs.RemoveAt(index);
            m_SpawnedEnemies.Add(enemy);
        }

        //Attente de la destruction des ennemis
        yield return new WaitUntil(() => m_SpawnedEnemies.Count == 0);
    }

    //Destruction d'un ennemi
    private void OnEnemyDestroyed (BasicEnemy pEnemy)
    {
        pEnemy.OnDestroyed -= OnEnemyDestroyed;
        m_SpawnedEnemies.Remove(pEnemy);
    }

#if UNITY_EDITOR
    //Destruction des ennemis spawnés
    [ContextMenu("Destroy Spawned Enemies")]
    private void DestroySpawnedEnemies ()
    {
        BasicEnemy[] currentEnemies = m_SpawnedEnemies.ToArray();
        for (int i = 0; i < currentEnemies.Length; i++)
        {
            Destroy(currentEnemies[i].gameObject);
        }
    }

    protected void Update ()
    {
        //Passage de la vague en cours
        if (Input.GetKeyDown(m_DebugSkipWaveKey))
        {
            DestroySpawnedEnemies();
        }
    }
#endif

    private void Start ()
    {
        //Initialisation
        StartCoroutine(SpawnWaves());
    }

#if UNITY_EDITOR
    protected void OnDrawGizmosSelected ()
    {
        //Rayon de spawn
        Gizmos.color    = Color.blue;
        int nbPoints    = 64;
        float angleStep = 2f * Mathf.PI / nbPoints;

        for (int i = 0; i < nbPoints; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;
            Vector3 pnt1 = transform.position + m_SpawnRadius * new Vector3(Mathf.Cos(angle1), 0f, Mathf.Sin(angle1));
            Vector3 pnt2 = transform.position + m_SpawnRadius * new Vector3(Mathf.Cos(angle2), 0f, Mathf.Sin(angle2));

            Gizmos.DrawLine(pnt1, pnt2);
        }
    }
#endif
}