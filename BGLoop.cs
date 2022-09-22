using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGLoop : MonoBehaviour
{
    Camera _mainCam;
    SpriteRenderer _spriteRenderer;

    [SerializeField]
    float _widthImage;
    // Start is called before the first frame update
    void Start()
    {
        _mainCam = Camera.main;
        _spriteRenderer = this.GetComponent<SpriteRenderer>();
        Texture image = _spriteRenderer.sprite.texture;

        _widthImage = image.width / _spriteRenderer.sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(_mainCam.transform.position.x - this.transform.position.x) > _widthImage)
        {
            float offset = Mathf.Abs(_mainCam.transform.position.x - this.transform.position.x) - _widthImage ;
            this.transform.position = new Vector2(_mainCam.transform.position.x +offset , this.transform.position.y);
            Debug.Log(offset);
        }
    }
}
