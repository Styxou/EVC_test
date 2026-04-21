using UnityEngine;

namespace Athena.Prototype
{
    

    public class UI : MonoBehaviour
    {
        [SerializeField] GameObject g_piece;
        Score score;
        void Start() 
        {
            score = GameObject.Find("Player").gameObject.GetComponent<Score>();
        }

        private void OnTriggerEnter(Collider other)
        {
            score.addScore();
            Destroy(g_piece.gameObject);
        }

    }


}
