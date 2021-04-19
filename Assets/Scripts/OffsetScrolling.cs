using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetScrolling : MonoBehaviour
{
    [SerializeField]
    public float _scrollSpeed;

    private Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        _renderer.material.mainTextureOffset = new Vector2(Time.time * _scrollSpeed, 0);
    }
}