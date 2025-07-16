using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerArea : MonoBehaviour
{
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public bool PlayerInRange()
    {
        // Get the center and size of the box in world space
        Vector3 boxCenter = boxCollider.bounds.center;
        Vector3 boxSize = boxCollider.bounds.size;

        // Check for all colliders within the box
        Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize / 2, Quaternion.identity);

        // Check if any of the colliders is tagged "Player"
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true; // Player is inside
            }
        }
        return false; // Player is not inside
    }

    void OnDrawGizmos()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            // Set Gizmo color
            Gizmos.color = new Color(0, 1, 0, 0.4f); // Green with transparency

            // Draw the OverlapBox area
            Gizmos.DrawWireCube(boxCollider.bounds.center, boxCollider.bounds.size);
        }
    }
}
