using UnityEngine;

public class FireBallScript : MonoBehaviour
{
    public float movementSpeed = 10;

    Transform t;

    private bool hit;

    private void Awake()
    {
        t = transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (hit)
            return; 
        t.position += Time.deltaTime * movementSpeed * t.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hit)
            return;
        hit = true;
        Destroy(this.gameObject,2);   
    }
}
