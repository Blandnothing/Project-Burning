using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{

    //�������ƶ�����
    public float speed = 1f;

    //����ͼ���
    private float textUnitSizex;

    //��ʼƫ���
    float beginOffset;


    // Start is called before the first frame update
    void Start()
    {
        //��ȡ����ͼ�����
        Sprite sprite = transform.GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textUnitSizex = transform.localScale.x * texture.width / sprite.pixelsPerUnit;//����ͼ��ȳ����ؿ��
        beginOffset = transform.position.x - Camera.main.transform.position.x;

        EventCenter.Instance.AddEvent<float>(EventName.playerMoveX, Move);

    }
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEvent<float>(EventName.playerMoveX,Move);
    }

    // Update is called once per frame
    public void Move(float offset)
    {
        transform.Translate(offset * speed * Vector2.left);

        float distance = transform.position.x - beginOffset - Camera.main.transform.position.x;
        if (Mathf.Abs(distance) >textUnitSizex)
        {
            //ƫ����
            float offsetPositionX = Camera.main.transform.position.x + beginOffset + distance%textUnitSizex;
            offsetPositionX -= offset % (distance % textUnitSizex);
            //�ƶ�����ͼ
            transform.position = new Vector3(offsetPositionX, transform.position.y);
        }
    }
}
