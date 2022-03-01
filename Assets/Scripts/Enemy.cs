using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int bulletSpeed;

    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites; // Basic Image 0, Hit Image 1

    float maxShotDelay;
    float curShotDelay;

    float maxDropDelay;
    float curDropDelay;

    public GameObject bulletObjA;
    public GameObject bulletObjB;

    public GameObject ItemBoom;
    public GameObject ItemCoin;
    public GameObject ItemPower;

    public GameObject player;
    public GameManager gameManager;
    public ObjectManager objectManager;

    SpriteRenderer spriteRenderer;
    Animator anim;

    public int patternIndex;
    public int curPatternCount;
    public int[] maxPatternCount;

    public bool bossActive = false;

    void Awake()
    {
        if(enemyName == "B")
            anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        bulletSpeed = 4;
        maxShotDelay = 2.5f;
        maxDropDelay = 7;
    }
    void OnEnable()
    {
        switch (enemyName)
        {
            case "B":
                bossActive = false;
                health = 2000;
                Invoke("Stop", 2f);
                break;
            case "L":
                health = 45;
                break;
            case "M":
                health = 12;
                break;
            case "S":
                health = 3;
                break;
        }
    }
    void Update()
    {
        if (enemyName == "B")
        {
            Boss();
            return;
        }

        Fire();
        Reload();
    }

    void Boss()
    {
        curDropDelay += Time.deltaTime;
        if(bossActive && gameObject.activeSelf)
           gameManager.bossHpbar.HandleHp((float)health);

        if (curDropDelay > maxDropDelay)
            ItemDrop();
    }

    void Stop()
    {
        if (!gameObject.activeSelf)
            return;

        gameManager.bossHpbar.maxHp = health;
        gameManager.bossHpbar.gameObject.SetActive(true);
        bossActive = true;
        Rigidbody2D rigid = GetComponent<Rigidbody2D>();
        rigid.velocity = Vector2.zero;

        Invoke("Think", 2);
    }

    void Think()
    {
        if (!gameObject.activeSelf)
            return;

        patternIndex = Random.Range(0, 4);
        curPatternCount = 0; // 패턴 변경 시, 패턴 실행 횟수 변수 초기화

        switch(patternIndex)
        {
            case 0:
                FireFoward();
                break;
            case 1:
                FireShot();
                break;
            case 2:
                FireArc();
                break;
            case 3:
                FireAround();
                break;
        }
    }

    void FireFoward()
    {
        if (!gameObject.activeSelf) return;

        //#. Fire 4 Bullet Foward
        GameObject bulletR = objectManager.MakeObj("BulletBossB");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletRR = objectManager.MakeObj("BulletBossB");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        GameObject bulletL = objectManager.MakeObj("BulletBossB");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletLL = objectManager.MakeObj("BulletBossB");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();

        rigidR.AddForce(Vector2.down * bulletSpeed*2.5f, ForceMode2D.Impulse);
        rigidRR.AddForce(Vector2.down * bulletSpeed*2.5f, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * bulletSpeed*2.5f, ForceMode2D.Impulse);
        rigidLL.AddForce(Vector2.down * bulletSpeed*2.5f, ForceMode2D.Impulse);

        //#.Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireFoward", 2f);
        else
            Invoke("Think", 3f);
    }

    void FireShot()
    {
        //#.Fire 5 Random Shotgun Bullet to Player
        for (int i=0; i < 5; i++)
        {
            if (!gameObject.activeSelf) return;

            GameObject bullet = objectManager.MakeObj("BulletEnemyB");
            bullet.transform.position = transform.position;
            
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = player.transform.position - transform.position;
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f, 2f));
            dirVec += ranVec;
            rigid.AddForce(dirVec.normalized * bulletSpeed*2, ForceMode2D.Impulse);
        }

        //#.Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireShot", 1f);
        else
            Invoke("Think", 3f);
    }

    void FireArc()
    {
        //#.Rire Arc Continue Fire
        for (int i = 0; i < 5; i++)
        {
            if (!gameObject.activeSelf) return;

            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 5 * curPatternCount / maxPatternCount[patternIndex]), -1);
            rigid.AddForce(dirVec.normalized * bulletSpeed * 1.7f, ForceMode2D.Impulse);
        }

        //#.Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireArc", 0.13f);
        else
            Invoke("Think", 3);
    }

    void FireAround()
    {
        //#.Fire Around
        int roundNumA = 45;
        int roundNumB = 50;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;
        for(int i = 0; i < roundNumA; i++)
        {
            if (!gameObject.activeSelf) return;

            GameObject bullet = objectManager.MakeObj("BulletBossA");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * i / roundNum)
                                        , Mathf.Sin(Mathf.PI * 2 * i / roundNum));

            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);

            Vector3 rotVec = Vector3.forward * 360 * i / roundNum + Vector3.forward * 90;
            
            bullet.transform.Rotate(rotVec); 
        }

        //#.Pattern Counting
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])
            Invoke("FireAround", 1.5f);
        else
            Invoke("Think", 3);
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)
            return;

       if(enemyName == "S")
        {
            GameObject bullet = objectManager.MakeObj("BulletEnemyA");
            bullet.transform.position = transform.position;
            
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            Vector3 dirVec = player.transform.position - transform.position;
            rigid.AddForce(dirVec.normalized * bulletSpeed, ForceMode2D.Impulse);
        }
       else if(enemyName == "L")
        {
            GameObject bulletR = objectManager.MakeObj("BulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            GameObject bulletL = objectManager.MakeObj("BulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f; 
            
            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);

            rigidR.AddForce(dirVecR.normalized * bulletSpeed, ForceMode2D.Impulse);
            rigidL.AddForce(dirVecL.normalized * bulletSpeed, ForceMode2D.Impulse);
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void ItemDrop()
    {
        int ran = Random.Range(0, 5);

        if (ran == 0)
        {
            GameObject itemBoom = objectManager.MakeObj("ItemBoom");
            itemBoom.transform.position = transform.position;
        }
        else
        {
            GameObject itemPower = objectManager.MakeObj("ItemPower");
            itemPower.transform.position = transform.position;
        }

        curDropDelay = 0;
    }

    public void OnHit(int dmg)
    {
        if (health <= 0)
            return;

        health -= dmg;

        if(enemyName == "B")
            anim.SetTrigger("OnHit"); // Hit Anim
        else
        {
            spriteRenderer.sprite = sprites[1]; // Hit Image
            Invoke("ReturnSprite", 0.1f);
        }

        if(health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();
            playerLogic.score += enemyScore;

            //#.Random Ratio Item Drop
            int ran = enemyName == "B" ? 0 : Random.Range(0, 10);

            if (ran < 5)
            {
                Debug.Log("Not Item");
            }
            else if (ran < 7) // Coin 20%
            {
                GameObject itemCoin = objectManager.MakeObj("ItemCoin");
                itemCoin.transform.position = transform.position;
            }
            else if (ran < 9) // Power 10%
            {
                GameObject itemPower = objectManager.MakeObj("ItemPower");
                itemPower.transform.position = transform.position;
            }
            else if (ran < 10) // Boom 10%
            {
                GameObject itemBoom = objectManager.MakeObj("ItemBoom");
                itemBoom.transform.position = transform.position;
            }

            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            gameManager.CallExplosion(transform.position, enemyName);

            //#.Boss Kill
            if (enemyName == "B")
            {
                curPatternCount = 0;
                gameManager.StageEnd();
            }
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "BorderBullet" && enemyName != "B")
        {
            gameObject.SetActive(false);
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            OnHit(bullet.dmg);

            collision.gameObject.SetActive(false);
        }
    }
}
