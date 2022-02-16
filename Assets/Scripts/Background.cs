using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int startIndex;
    [SerializeField] int endIndex;
    [SerializeField] Transform[] sprites;
    
    float veiwHeight;

    void Awake()
    {
        veiwHeight = Camera.main.orthographicSize * 2; // Camera Height
        startIndex = 2;
        endIndex = 0;
    }

    void Update()
    {
        Move();
        Scrolling();
    }
    void Move()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }
    void Scrolling()
    {
        if (sprites[endIndex].position.y < veiwHeight * (-1))
        {
            // #. Sprite ReUse
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            Vector3 frontSpritePos = sprites[endIndex].localPosition;
            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * veiwHeight;

            // #. Cursor Index Change
            int startIndexSave = startIndex;
            startIndex = endIndex;
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;
        }
    }
}
