using UnityEngine;

public class FireBallScript : MonoBehaviour
{
    public float movementSpeed = 10;

    Transform t;

    private bool hit;

    [HideInInspector]
    public AudioSource AudioSource;
    public AudioClip AudioClip;

    public FireBallShooter Shooter;

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

        if (other.tag == "Player" && other.GetComponentInChildren<FireBallShooter>() == Shooter)
            return;

        hit = true;
        Destroy(this.gameObject,2);   
        AudioSource.PlayOneShot(AudioClip);
    }
}
