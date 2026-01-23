using UnityEngine;

public class WinCube : MonoBehaviour
{
    public Transform nextCheckpoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.transform.parent.GetComponent<PlayerMovement>();
        Debug.Log("allo"+player);
        if (player != null)
        {
            other.transform.parent.position = nextCheckpoint.position;
        }
    }
}
