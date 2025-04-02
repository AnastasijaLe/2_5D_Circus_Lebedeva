using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 position;
    [SerializeField]private float maxRadForceVal, startRollingForce;
    float forceX, forceY, forceZ;
    public string diceFaceNum;
    public bool isLanded = false;
    public bool firstThrow = false;
     public bool rollCompleted = false;
    public bool rollConsumed = false;
    public bool inputEnabled = true;
    private AudioSource audioSource;


    void Awake()
    {
        Initialize(0);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputEnabled)
        return;

        if(rBody != null)
        {
            if(Input.GetMouseButton(0) && isLanded || Input.GetMouseButton(0) && !firstThrow) 
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit) ) 
                {
                    if(hit.collider != null && hit.collider.gameObject == this.gameObject) 
                    { 
                        firstThrow = true;
                        rollCompleted = false;
                        rollConsumed = false;
                        diceFaceNum = "";
                        RollDice();
                    }
                }
            }

            if (firstThrow && !isLanded &&
                rBody.velocity.magnitude < 0.05f &&
                rBody.angularVelocity.magnitude < 0.05f)
            {
                isLanded = true;
                rollCompleted = true;
                if (audioSource != null && audioSource.isPlaying)
                    audioSource.Stop();
            }
        }    
    }

    public void Initialize(int node) {
        if (node == 0) {
            rBody = GetComponent<Rigidbody>();
            position = transform.position;
        
        } else if(node == 1)
            transform.position = position;

        inputEnabled = true;
        firstThrow = false;
        isLanded = false;
        rollCompleted = false;
        rollConsumed = false;
        diceFaceNum = "";
        rBody.isKinematic = true;
        transform.rotation = new Quaternion(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360), 0);
    }

    private void RollDice() {
        rBody.isKinematic = false;

         if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
        

        forceX = Random.Range(0, maxRadForceVal);
        forceY = Random.Range(0, maxRadForceVal);
        forceZ = Random.Range(0, maxRadForceVal);
        rBody.AddForce(Vector3.up*Random.Range(800, startRollingForce));
        rBody.AddTorque(forceX, forceY, forceZ);  
    }

      private IEnumerator StopSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && audioSource.isPlaying){
            Debug.Log("STOP!");
            audioSource.Stop();
        }
            
    }

      public void ResetRollState()
    {
        firstThrow = false;
        isLanded = false;
        rollCompleted = false;
        rollConsumed = false;
        diceFaceNum = "";
        inputEnabled = true;
    }
}
