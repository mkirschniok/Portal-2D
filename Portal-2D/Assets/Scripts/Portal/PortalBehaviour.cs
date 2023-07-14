using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PortalBehaviour : MonoBehaviour
{
    Tilemap tilemap;
    Tilemap impostorTilemap;
    const int portalGridHeight = 6;
    const int portalGridWidth = 4;

    void Start()
    {
        tilemap = PortalManager.Instance.TilemapProperty;
        impostorTilemap = PortalManager.Instance.ImpostorTilemapProperty;

        MakeTilesBehindPortalNonCollidable();

        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("OpenPortal");
    }

    public void InitDestroyment()
    {
        RestoreTiles();
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("ClosePortal");
    }

    // wywo³ane na koniec animacji zamykania portalu
    void Destroy()
    {
        Destroy(this.gameObject);
    }

    void MakeTilesBehindPortalNonCollidable()
    {
        Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(transform.position);

        Debug.Log("grid pos: " + cellPosition);

        // przesuñ punkt startowy o 3 w dó³ w przestrzeni lokalnej
        cellPosition += Vector3Int.RoundToInt(gameObject.transform.up * -(portalGridHeight / 2));

        Vector3Int tilePos = Vector3Int.zero;

        // przesuñ punkt startowy ³¹cznie o 2 jednostki w bok
        for (int x = 0; x < portalGridWidth; ++x)
        {
            Vector3Int shiftX = Vector3Int.RoundToInt(gameObject.transform.right * x);
            // przesuñ punkt startowy ³¹cznie o 6 jednostek w górê
            for (int y = 0; y < portalGridHeight; ++y)
            {
                Vector3Int shiftY = Vector3Int.RoundToInt(gameObject.transform.up * y);

                tilePos = cellPosition + shiftX + shiftY;// + new Vector3Int(x, y, 0);
                var tile = tilemap.GetTile(tilePos);
                impostorTilemap.SetTile(tilePos, tile);
                tilemap.SetTile(tilePos, null);
            }
        }
        // ------- TO FIX ------------
        if (Mathf.Abs(transform.eulerAngles.z - 270) < 0.01)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }
        else if (transform.eulerAngles.z != 0)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
        }

    }

    void RestoreTiles()
    {
        // ------- TO FIX ------------
        if (Mathf.Abs(transform.eulerAngles.z - 270) < 0.01)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }
        else if (transform.eulerAngles.z != 0)
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
        }


        Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(transform.position);
        cellPosition += Vector3Int.RoundToInt(gameObject.transform.up * -(portalGridHeight / 2));
        Vector3Int tilePos = Vector3Int.zero;

        for (int x = 0; x < portalGridWidth; ++x)
        {
            Vector3Int shiftX = Vector3Int.RoundToInt(gameObject.transform.right * x);
            for (int y = 0; y < portalGridHeight; ++y)
            {
                Vector3Int shiftY = Vector3Int.RoundToInt(gameObject.transform.up * y);

                tilePos = cellPosition + shiftX + shiftY;// + new Vector3Int(x, y, 0);
                var tile = impostorTilemap.GetTile(tilePos);
                tilemap.SetTile(tilePos, tile);
                impostorTilemap.SetTile(tilePos, null);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PortalManager.Instance.ObjectToCloneProperty = collision.gameObject;
        PortalManager.Instance.CreateClone(gameObject, collision.gameObject);
        Debug.Log(gameObject.name);
    }

    void OnCollisionEnter2D(Collision2D collider)
    {
        PortalManager.Instance.Teleport();
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        PortalManager.Instance.DestroyClone(collider.gameObject);
    }
}