using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MirrorCube : MonoBehaviour
{
    [SerializeField] AudioSource cubeSound;
    [SerializeField] float detectionRadius = 4f;
    [SerializeField] private LayerMask layerMask;  // Warstwa obiekt�w, kt�re mog� zablokowa� laser
    TMP_Text promptText;
    bool taken = false;
    Rigidbody2D rigidbody2D;
    static bool promptWasDisplayed = false;
    private LineRenderer lineRenderer;
    public SpriteRenderer spriteRenderer;
    float backupMass = 0;
    public Sprite mirrorOnSprite;
    public Sprite mirrorOffSprite;
    Vector3 start;      // Punkt pocz�tkowy lasera
    Vector3 maxEnd;     // Punkt ko�cowy lasera (maksymalny zasi�g)
    Vector3 realEnd;        // Punkt ko�cowy lasera (aktualny zasi�g)

    public void Take()
    {
        if (taken) return;
        backupMass = rigidbody2D.mass;
        rigidbody2D.mass = 0.008f;
        rigidbody2D.gravityScale = 0;
        taken = true;
        UnityEngine.Debug.Log("Podnosze lustro (lustro)");
    }

    public void Drop()
    {
        if (!taken) return;
        rigidbody2D.mass = backupMass;
        rigidbody2D.gravityScale = 1;
        taken = false;
    }

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();     // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;
        stopLaser();    // Wy��czenie lasera
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        start = transform.position;  // Pierwszy punkt to pozycja transmitera
        maxEnd = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z); // Odleg�y punkt na lewo od kostki
        realEnd = maxEnd;   // Pocz�tkowo ustawiamy punkt ko�cowy na maksymalny zasi�g
        if (lineRenderer.enabled)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, (maxEnd - start).normalized, Vector3.Distance(start, maxEnd), layerMask);
            if (hit.collider != null)
            {
                realEnd = hit.point;
            }
            lineRenderer.SetPosition(0, start);     // Ustawienie pierwszego punktu linii
            lineRenderer.SetPosition(1, realEnd);   // Ustawienie drugiego punktu linii
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        cubeSound.Play();
    }

    public void startLaser()
    {
        // Zmiana sprite'a na aktywowany
        spriteRenderer.sprite = mirrorOnSprite;
        lineRenderer.enabled = true;    // W��czenie linii
    }

    public void stopLaser()
    {
        // Zmiana sprite'a na nieaktywowany
        spriteRenderer.sprite = mirrorOffSprite;
        lineRenderer.enabled = false;   // Wy��czenie linii
    }
}
