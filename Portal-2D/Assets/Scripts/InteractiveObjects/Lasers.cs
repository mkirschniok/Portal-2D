using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Klasa Lasers odpowiada za zachowanie laser�w 
/// </summary>
public class Lasers : MonoBehaviour
{
    /// <summary>
    /// D�wi�k lasera
    /// </summary>
    [SerializeField] AudioSource laserSound;
    /// <summary>
    /// D�wi�k w��czenia lasera
    /// </summary>
    [SerializeField] AudioSource laserOn;
    /// <summary>
    /// D�wi�k wy��czenia lasera
    /// </summary>
    [SerializeField] AudioSource laserOff;
    /// <summary>
    /// Zdarzenie wywo�ywane, gdy laser trafia w odbiornik
    /// </summary>
    [SerializeField] UnityEvent onReceiverHit;
    /// <summary>
    /// Zdarzenie wywo�ywane, gdy laser znika z odbiornika
    /// </summary>
    [SerializeField] UnityEvent onReceiverReleased;
    /// <summary>
    /// Warstwa obiekt�w, kt�re mog� zablokowa� laser
    /// </summary>
    [SerializeField] LayerMask layerMask;
    /// <summary>
    /// Domy�lny sprite
    /// </summary>
    public Sprite defaultSprite;
    /// <summary>
    /// Aktywowany sprite (po w��czeniu lasera lub po trafieniu w odbiornik)
    /// </summary>
    public Sprite activatedSprite;
    /// <summary>
    /// Wektor z pocz�tkowym punktem lasera
    /// </summary>
    public Vector3 start;
    /// <summary>
    /// Wektor z odleg�ym punktem na prostej lasera
    /// </summary>
    public Vector3 maxEnd;
    /// <summary>
    /// Wektor z rzeczywistym ko�cem lasera (po wykryciu kolizji)
    /// </summary>
    public Vector3 realEnd;
    /// <summary>
    /// Komponent LineRenderer - do rysowania lasera
    /// </summary>
    private LineRenderer lineRenderer;
    /// <summary>
    /// Komponent SpriteRenderer - do zmiany sprite'�w
    /// </summary>
    private SpriteRenderer spriteRenderer;


    /// <summary>
    /// Metoda wywo�ywana przed pierwszym od�wie�eniem klatki
    /// </summary>
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    // Pobranie komponentu SpriteRenderer
        lineRenderer = GetComponent<LineRenderer>();        // Pobranie komponentu LineRenderer
        lineRenderer.positionCount = 2;                     // Ustawienie liczby punkt�w na 2
        start = transform.position;                         // Pierwszy punkt to pozycja transmitera
        stopLaser();                                        // Wy��czenie lasera na pocz�tku gry
        // Ustawienie w�a�ciwego kierunku i zwrotu lasera w zale�no�ci od rotacji transmitera
        if (Math.Round(transform.rotation.z, 1) == -0.7)
        {
            maxEnd = new Vector3(transform.position.x, transform.position.y - 100, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 0.7)
        {
            maxEnd = new Vector3(transform.position.x, transform.position.y + 100, transform.position.z);
        }
        else if (transform.rotation.z == 0.0)
        {
            maxEnd = new Vector3(transform.position.x + 100, transform.position.y, transform.position.z);
        }
        else if (Math.Round(transform.rotation.z, 1) == 3.1)
        {
            maxEnd = new Vector3(transform.position.x - 100, transform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// Metoda powoduj�ca uruchomienie lasera
    /// </summary>
    public void startLaser()
    {
        // Zmiana sprite'a na aktywowany
        spriteRenderer.sprite = activatedSprite;
        lineRenderer.enabled = true;    // W��czenie linii
        laserOn.Play();
        laserSound.mute = false;
        laserSound.PlayDelayed(laserOn.clip.length);
        laserSound.loop = true;
    }

    /// <summary>
    /// Metoda powoduj�ca zatrzymanie lasera
    /// </summary>
    public void stopLaser()
    {
        // Zmiana sprite'a na domy�lny
        spriteRenderer.sprite = defaultSprite;
        lineRenderer.enabled = false;   // Wy��czenie linii
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
        GameObject.Find("Mirror").GetComponent<MirrorCube>().stopLaser();
        laserSound.mute = true;
        laserSound.loop = false;
        if (DoorOut.isActive) GameObject.Find("DoorOut").GetComponent<DoorOut>().CloseDoor();
        laserOff.Play();
    }

    /// <summary>
    /// Metoda wywo�ywana co klatk�
    /// </summary>
    void Update()
    {
        if (lineRenderer.enabled)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, (maxEnd - start).normalized, Vector3.Distance(start, maxEnd), layerMask);
            if (hit.collider != null)
            {
                realEnd = hit.point;
            }
            lineRenderer.SetPosition(0, start); // Ustawienie pierwszego punktu linii
            lineRenderer.SetPosition(1, realEnd);   // Ustawienie drugiego punktu linii
            if (hit.collider != null && hit.collider.gameObject.tag == "Mirror")
            {
                hit.collider.gameObject.GetComponent<MirrorCube>().startLaser();
            }
            else
            {
                GameObject.Find("Mirror").GetComponent<MirrorCube>().stopLaser();
                if (hit.collider != null && hit.collider.gameObject.tag == "Receiver")
                {
                    hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite = activatedSprite;
                    if (!DoorOut.isActive) onReceiverHit.Invoke();
                }
                else
                {
                    if (DoorOut.isActive) onReceiverReleased.Invoke();
                    GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
                    if (hit.collider != null && hit.collider.gameObject.tag == "Blue Portal")
                    {
                        Debug.Log("The blue portal was hit by laser");
                        PortalLaser.isBluePortalHit = true;
                    }
                    else if (hit.collider != null && hit.collider.gameObject.tag == "Orange Portal")
                    {
                        Debug.Log("The orange portal was hit by laser");
                        PortalLaser.isOrangePortalHit = true;
                    }
                    else
                    {
                        PortalLaser.isBluePortalHit = false;
                        PortalLaser.isOrangePortalHit = false;
                    }
                }
            }
        }
        else
        {
            PortalLaser.isBluePortalHit = false;
            PortalLaser.isOrangePortalHit = false;
        }
    }

    /// <summary>
    /// Metoda wywo�ywana, gdy laser trafia w odbiornik - zmienia sprite odbiornika na aktywowany
    /// </summary>
    public void ReceiverHit()
    {
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = activatedSprite;
    }

    /// <summary>
    /// Metoda wywo�ywana, gdy laser znika z odbiornika - zmienia sprite odbiornika na domy�lny
    /// </summary>
    public void ReceiverReleased()
    {
        GameObject.Find("LaserReceiver").GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }
}
