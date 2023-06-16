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
    const int portalGridWidtht = 2;

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

    // wywo�ane na koniec animacji zamykania portalu
    void Destroy()
    {
        Destroy(this.gameObject);
    }

    void MakeTilesBehindPortalNonCollidable()
    {
        Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(transform.position);

        Debug.Log("grid pos: " + cellPosition);

        // przesu� punkt startowy o 3 w d� w przestrzeni lokalnej
        cellPosition += Vector3Int.RoundToInt(gameObject.transform.up * -(portalGridHeight / 2));

        Vector3Int tilePos = Vector3Int.zero;

        // przesu� punkt startowy ��cznie o 2 jednostki w bok
        for (int x = 0; x < portalGridWidtht; ++x)
        {
            Vector3Int shiftX = Vector3Int.RoundToInt(gameObject.transform.right * x);
            // przesu� punkt startowy ��cznie o 6 jednostek w g�r�
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
        if (transform.rotation.z != 0)
        {
            transform.position = new Vector3(transform.position.x + 1, transform.position.y + 1, transform.position.z);
        }
    }

    void RestoreTiles()
    {
        // ------- TO FIX ------------
        if (transform.rotation.z != 0)
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y - 1, transform.position.z);
        }
        Vector3Int cellPosition = tilemap.layoutGrid.WorldToCell(transform.position);
        cellPosition += Vector3Int.RoundToInt(gameObject.transform.up * -(portalGridHeight / 2));
        Vector3Int tilePos = Vector3Int.zero;

        for (int x = 0; x < portalGridWidtht; ++x)
        {
            Vector3Int shiftX = Vector3Int.RoundToInt(gameObject.transform.right * x);
            for (int y = 0; y < portalGridHeight; ++y)
            {
                Vector3Int shiftY = Vector3Int.RoundToInt(gameObject.transform.up * y);

                tilePos = cellPosition + shiftX + shiftY;// + new Vector3Int(x, y, 0);
                var tile = impostorTilemap.GetTile(tilePos);
                impostorTilemap.SetTile(tilePos, null);
                tilemap.SetTile(tilePos, tile);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PortalManager.Instance.CreateClone(gameObject, collision.gameObject);
        Debug.Log(this.gameObject.name);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PortalManager.Instance.Teleport();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PortalManager.Instance.DestroyClone(collision.gameObject);
    }
}