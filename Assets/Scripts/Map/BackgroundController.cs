using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    //子物体移动倍速
    public float speed = 1f;

    //纹理图宽度
    private float textUnitSizex;

    //开始偏差长度
    float beginOffset;


    // Start is called before the first frame update
    void Start()
    {
        //获取纹理图及宽度
        Sprite sprite = transform.GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textUnitSizex = transform.localScale.x * texture.width / sprite.pixelsPerUnit;//纹理图宽度除像素宽度
        beginOffset = transform.position.x - Camera.main.transform.position.x;

        EventCenter.Instance.AddEvent<Vector2>(EventName.playerMoveX, Move);

    }
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEvent<Vector2>(EventName.playerMoveX,Move);
    }

    // Update is called once per frame
    public void Move(Vector2 offset)
    {
        transform.Translate(new Vector2(offset.x * speed,offset.y));

        float distance = transform.position.x - beginOffset - Camera.main.transform.position.x;
        if (Mathf.Abs(distance) >textUnitSizex)
        {
            //偏移量
            float offsetPositionX = Camera.main.transform.position.x + beginOffset + distance%textUnitSizex;
            offsetPositionX -= offset.x % (distance % textUnitSizex);
            //移动背景图
            transform.position = new Vector3(offsetPositionX, transform.position.y);
        }
    }
}
