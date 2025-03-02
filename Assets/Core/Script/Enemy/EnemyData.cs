using UnityEngine;


    [CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
    public class EnemyData : ScriptableObject
    {
        [Header("References")]
        public GameObject Prefab;

        [Header("characteristics")]
        [Range(1,100)]
        public int health = 10;
        [Range(1, 50)]
        public int AttackPower = 2;
        [Range(1, 20)]
        public float AttackCouldown = 10;
        [Range(1, 60)]
        public int speed = 10;
    }