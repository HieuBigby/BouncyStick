using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColliderFitSprite : MonoBehaviour
{
    void Update()
    {
        var _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            _collider = gameObject.AddComponent<BoxCollider2D>();
        }

        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        _collider.offset = combinedBounds.center - transform.position;
        _collider.size = new Vector2(combinedBounds.size.x / transform.lossyScale.x,
                                     combinedBounds.size.y / transform.lossyScale.y);
    }
}
