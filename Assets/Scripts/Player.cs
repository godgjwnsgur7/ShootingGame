using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;

    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        speed = 5;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal"); // ����(x��)
        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1)) h = 0;

        float v = Input.GetAxisRaw("Vertical"); // ���� (y��)
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1)) v = 0;

        Vector3 curPos = transform.position; // ���� ��ġ ������
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime; // �̵��� �ӵ�

        transform.position = curPos + nextPos; // �̵�

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
            anim.SetInteger("Input", (int)h); // ���� �� ��ȯ
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")
        {
            switch(collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.gameObject.name)
        {
            case "Top":
                isTouchTop = false;
                break;
            case "Bottom":
                isTouchBottom = false;
                break;
            case "Left":
                isTouchLeft = false;
                break;
            case "Right":
                isTouchRight = false;
                break;
        }
    }
}
