using System.Threading.Tasks;
using UnityEngine;

public class targetInfoScript : MonoBehaviour
{
    public TargetHealthBarScript targetHealthBarScript;

    KillableScript target;
    // Start is called before the first frame update
    void Start()
    {
        this.targetHealthBarScript.Hide();
    }

    private void FixedUpdate()
    {
        if (target is not null)
            return;
            
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + Vector3.up +transform.forward * 4, 10);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.transform == this.transform)
                continue;
            var killable = hitCollider.GetComponent<KillableScript>();
            if (killable is not null)
                SetNewTarget(killable);
        }
    }

    private void SetNewTarget(KillableScript killable)
    {
        this.target = killable;
        this.targetHealthBarScript.SetHealth(target.health.Value);
        this.targetHealthBarScript.SetName(target.killablename);
        this.targetHealthBarScript.Show();

        target.health.OnValueChanged += UpdateHealth;

        LoseTargetWhenNotInRange();
    }

    async Task LoseTargetWhenNotInRange()
    {

        while(target is not null)
        {
            await Task.Delay(1000);
            if(Vector3.Distance(this.transform.position, target.transform.position) > 15)
            {
                LoseTarget();
            }
        }
    }

    private void UpdateHealth(int previousValue, int newValue)
    {
        this.targetHealthBarScript.SetHealth(newValue);
    }

    private void LoseTarget()
    {
       this.target.health.OnValueChanged -= UpdateHealth;
       this.targetHealthBarScript.Hide();
        this.target = null;
    }

    

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + Vector3.up + transform.forward * 4, 10);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
