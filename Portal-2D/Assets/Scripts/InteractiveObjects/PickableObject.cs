using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class PickableObject : MonoBehaviour , IPortalEventsListener
{
    [SerializeField] protected AudioSource cubeSound;
    [SerializeField] protected float detectionRadius = 4f;
    TMP_Text promptText;
    protected bool taken = false;
    protected Rigidbody2D rigidbody2D;
    static bool promptWasDisplayed = false;

    float backupMass = 0;
    Transform attachpoint;

    public void Take(Transform attach)
    {
        if (taken || !attach)
            return;

        backupMass = rigidbody2D.mass;
        rigidbody2D.mass = 0.008f;
        rigidbody2D.gravityScale = 0;
        taken = true;
        attachpoint = attach;
        UnityEngine.Debug.Log("Podnosze kostke (kostka)");

    }

    public void Drop()
    {
        if (!taken)
            return;
        rigidbody2D.mass = backupMass;
        rigidbody2D.gravityScale = 1;
        taken = false;
        attachpoint = null;
    }

    protected virtual void Start()
    {
        GameObject textObject = GameObject.FindGameObjectWithTag("PromptText");
        if (textObject != null)
        {
            promptText = textObject.GetComponent<TMP_Text>();
        }
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (attachpoint != null)
        {
            var cubePosition = transform.position;
            var targetPos    = attachpoint.transform.position;
            var diff         = (targetPos - cubePosition)*10;
            GetComponent<Rigidbody2D>().velocity = new Vector2(diff.x, diff.y);
        }

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

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (cubeSound!=null)
            cubeSound.Play();
    }

    public virtual void OnTeleported(PortalLogic srcPortal, PortalLogic dstPortal)
    {
        Drop();
    }
    public virtual void OnExitedPortalArea(PortalLogic portal)
    {
    }
}