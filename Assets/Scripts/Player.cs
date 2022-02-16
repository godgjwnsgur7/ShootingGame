using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life;
    public int score;
    public int power;
    public int boom;

    int bulletSpeed;
    int vector_h;

    int speed;
    int maxPower;
    int maxBoom;
    float maxShotDelay;
    float curShotDelay;

    bool isTouchTop;
    bool isTouchBottom;
    bool isTouchLeft;
    bool isTouchRight;

    public bool isBoomTime;
    public bool isHit;
    public bool isRespawnTime;
    public bool isGameStop;

    public bool isButtonA;
    bool isButtonB;

    [SerializeField] GameObject BoomEffect;

    [SerializeField] GameManager gameManager;
    [SerializeField] ObjectManager objectManager;
    [SerializeField] Joystick joystick;

    [SerializeField] GameObject[] followers;

    Animator anim;
    SpriteRenderer spriteRenderer;
    

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetPlayer();
    }
    void OnEnable()
    {
        if(power != 1)
            power--;
        
        if (power > 4)
            power = 4;

        if(life == 3)
            return;
        Unbeatable();
        Invoke("Unbeatable", 2f);
    }

    void Update()
    {
        if (isGameStop)
            return;
        Move();
        Fire();
        Reload();
        Boom();
    }

    void SetPlayer()
    {
        life = 3;
        bulletSpeed = 7;
        speed = 4;
        power = 1;
        maxPower = 7;
        boom = 0;
        maxBoom = 3;
        maxShotDelay = 0.15f;
        vector_h = 0;
    }

    public void PlayerReset()
    {
        if (power != 1)
            power--;

        if (power > 4)
            power = 4;

        for (int i = 0; i < 3; i++)
            followers[i].SetActive(false);

        boom = 0;
        gameManager.UpdateBoomIcon(boom);
        isRespawnTime = true;
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;

        if(isRespawnTime) // Invincibility Time On
        {
            isRespawnTime = true;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
        else // Invincibility Time Off
        {
            isRespawnTime = false;
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }

    void Move()
    {
        //#. Keyboard Control Value
        // float h = Input.GetAxisRaw("Horizontal");
        // float v = Input.GetAxisRaw("Vertical");
        
        //#. Joy Control Value
        float h = joystick.Horizontal();
        float v = joystick.Vertical();

        if (h > 0.4f)
            vector_h = 1;
        else if (h < -0.4f)
            vector_h = -1;
        else
            vector_h = 0;

        if ((isTouchRight && h > 0f) || (isTouchLeft && h < 0f))
            h = 0;
        if ((isTouchTop && v > 0f) || (isTouchBottom && v < 0f))
            v = 0;

        anim.SetInteger("Input", vector_h);

        if ( h != 0 || v != 0)
            transform.position += new Vector3(h, v, 0) * speed * Time.deltaTime;
    }

    public void ButtonADown()
    {
        isButtonA = true;
    }
    public void ButtonAUp()
    {
        isButtonA = false;
    }
    public void ButtonBDown()
    {
        if (boom == 0) return;
        isButtonB = true;
    }

    void Fire()
    {
        // if (!Input.GetButton("Fire1")) return;

        if (!isButtonA) return;

        if (curShotDelay < maxShotDelay) return;

        switch (power)
        {
            case 1: // Power 1
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                break;
            case 2: // Power 2
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f; 
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                break;
            case 3: // Power 3
                GameObject bulletB = objectManager.MakeObj("BulletPlayerB");
                bulletB.transform.position = transform.position;
                Rigidbody2D rigidB = bulletB.GetComponent<Rigidbody2D>();
                rigidB.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                break;
            default: // Power 4~7
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
                break;
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }


    void Boom()
    {
        // if (!Input.GetButton("Fire2")) return;

        if (!isButtonB) return;

        if (isBoomTime || boom == 0) return;

        isButtonB = false;
        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        //#1.Effect visible
        BoomEffect.SetActive(true);
        Invoke("OffBoomEffect", 2f);

        gameManager.Remove_Enemy();
        gameManager.Remove_EnemyBullet();
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
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if (isHit || isRespawnTime) return;
            isHit = true;

            gameManager.CallExplosion(transform.position, "P");

            life--;
            gameManager.UpdateLifeIcon(life);
            
            boom = 0;
            gameManager.UpdateBoomIcon(boom);
            
            for(int i =0; i < 3; i++)
                followers[i].SetActive(false);

            if (life == 0)
                gameManager.GameOver();
            else
                gameManager.RespawnPlayer();

            gameObject.SetActive(false);

            if (collision.gameObject.name == "Enemy Boss(Clone)")
                return;
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch(item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (power == 5)
            followers[0].SetActive(true);
        else if (power == 6)
            followers[1].SetActive(true);
        else if (power == 7)
            followers[2].SetActive(true);
    }

    void OffBoomEffect()
    {
        BoomEffect.SetActive(false);
        isBoomTime = false;
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
