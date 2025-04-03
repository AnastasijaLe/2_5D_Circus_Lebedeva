using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideDetectorScript : MonoBehaviour
{
    DiceRollScript diceRollScript;
     AudioSource audioSource;

    void Awake()
    {
        diceRollScript = FindObjectOfType<DiceRollScript>();
        if(diceRollScript != null)
            audioSource = diceRollScript.GetComponent<AudioSource>();
    }

    private void OnTriggerStay(Collider other) {
        if (diceRollScript != null)
        {
            Rigidbody rb = diceRollScript.GetComponent<Rigidbody>();
            // Use magnitude threshold to decide when the dice has settled.
            if (rb.velocity.magnitude < 0.05f)
            {
                diceRollScript.isLanded = true;
                diceRollScript.rollCompleted = true;
                diceRollScript.diceFaceNum = other.name;
                // Stop the dice roll sound here.
                if (audioSource != null && audioSource.isPlaying)
                    audioSource.Stop();
            }
            else
            {
                diceRollScript.isLanded = false;
            }
        }
        else
        {
            Debug.LogError("DiceRollScript not found in scene!");
        }
    }    
}
