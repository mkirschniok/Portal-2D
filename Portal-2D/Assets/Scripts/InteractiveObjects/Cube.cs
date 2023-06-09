using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cube : MonoBehaviour
{
    [SerializeField] float detectionRadius = 4f;
    TMP_Text promptText;
    public static bool taken = false;
    static bool promptWasDisplayed = false;

    void Start()
    {
        GameObject textObject = GameObject.FindGameObjectWithTag("PromptText");
        if (textObject != null)
        {
            promptText = textObject.GetComponent<TMP_Text>();
        }
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        
        if (taken)
        {
            promptWasDisplayed= true;
        }

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player") && !taken && !promptWasDisplayed)
            {
                promptText.enabled = true;
                promptText.text = "Press   E   to  pickup";
                break;
            }
            else if (promptText.enabled)
            { 
                promptText.enabled = false;
                promptText.text = "";
            }
        }

       
    }

    
}
