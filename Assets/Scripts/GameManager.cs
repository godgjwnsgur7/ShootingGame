using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public int maxScore;
    public int curScore;

    public int stage;
    public int maxStage;
    public Animator stageAnim;
    public Animator clearAnim;
    public Animator fadeAnim;
    public Transform playerPos;

    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject GameOverSet;
    public ObjectManager objectManager;
    public Player playerLogic;
    public BossHpbar bossHpbar;

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;


    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };
        stage = 1;
        maxStage = 3;
        nextSpawnDelay = 2f;
        StageStart();
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)
        {
            SpawnEnemy();
            curSpawnDelay = 0;
        }

        //#. UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void ReadSpawnFile()
    {
        // #1.Init variable
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        Debug.Log("ReadSpawnFile");

        // #2.Read Respawn File
        TextAsset textFile = Resources.Load("Stage " + stage.ToString()) as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        // #3.Save Data
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == null)
                break;

            //#.Create Respawn Data
            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        // #.Close Text File
        stringReader.Close();

        // #.Fisrt Spawn delay
        nextSpawnDelay = spawnList[0].delay;
    }

    void SpawnEnemy()
    {
        int enemyIndex = 0;
        switch(spawnList[spawnIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }

        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; // 적 생성후 플레이어 변수를 넘겨줌
        enemyLogic.gameManager = this;
        enemyLogic.objectManager = objectManager;

        if(enemyPoint == 5 || enemyPoint == 6) //#.Right Spawn
        {
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
            enemy.transform.Rotate(Vector3.back * 90);
        }
        else if(enemyPoint == 7 || enemyPoint == 8) //#.Left Spawn
        {
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
            enemy.transform.Rotate(Vector3.forward * 90);
        }
        else // #. Front Spawn
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }

        //#. Increase Respawn Index
        spawnIndex++;
        if(spawnIndex == spawnList.Count)
        {
            spawnEnd = true;
            return;
        }

        //#. Update Next Respawn Delay
        nextSpawnDelay = spawnList[spawnIndex].delay;
    }

    public void UpdateLifeIcon(int life)
    {
        //#.UI Life Init Disable
        for (int i = 0; i < 3; i++)
            lifeImage[i].color = new Color(1, 1, 1, 0); // 투명

        //#.UI Life Active
        for (int i = 0; i < life; i++)
            lifeImage[i].color = new Color(1, 1, 1, 1); // 반투명
    }
    public void UpdateBoomIcon(int boom)
    {
        //#.UI Boom Init Disable
        for (int i = 0; i < 3; i++)
            boomImage[i].color = new Color(1, 1, 1, 0); // 투명

        //#.UI Boom Active
        for (int i = 0; i < boom; i++)
            boomImage[i].color = new Color(1, 1, 1, 1); // 반투명
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 3.5f;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void CallExplosion(Vector3 pos, string type)
    {
        GameObject explosion = objectManager.MakeObj("Explosion");
        Explosion explosionLogic = explosion.GetComponent<Explosion>();

        explosion.transform.position = pos;
        explosionLogic.StartExplosion(type);
    }

    public void Remove_Enemy()
    {
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int i = 0; i < enemiesL.Length; i++)
        {
            if (enemiesL[i].activeSelf)
            {
                Enemy enemyLogic = enemiesL[i].GetComponent<Enemy>();
                enemyLogic.OnHit(50);
            }
        }
        for (int i = 0; i < enemiesM.Length; i++)
        {
            if (enemiesM[i].activeSelf)
            {
                Enemy enemyLogic = enemiesM[i].GetComponent<Enemy>();
                enemyLogic.OnHit(50);
            }
        }
        for (int i = 0; i < enemiesS.Length; i++)
        {
            if (enemiesS[i].activeSelf)
            {
                Enemy enemyLogic = enemiesS[i].GetComponent<Enemy>();
                enemyLogic.OnHit(50);
            }
        }
    }

    public void Remove_EnemyBullet()
    {
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        GameObject[] bulletsC = objectManager.GetPool("BulletBossA");
        GameObject[] bulletsD = objectManager.GetPool("BulletBossB");

        for (int i = 0; i < bulletsA.Length; i++)
        {
            if (bulletsA[i].activeSelf)
                bulletsA[i].SetActive(false);
        }
        for (int i = 0; i < bulletsB.Length; i++)
        {
            if (bulletsB[i].activeSelf)
                bulletsB[i].SetActive(false);
        }
        for (int i = 0; i < bulletsC.Length; i++)
        {
            if (bulletsC[i].activeSelf)
                bulletsC[i].SetActive(false);
        }
        for (int i = 0; i < bulletsD.Length; i++)
        {
            if (bulletsD[i].activeSelf)
                bulletsD[i].SetActive(false);
        }

        Invoke("Remove_EnemyBulletExe", 0.1f);
    }

    void Remove_EnemyBulletExe()
    {
        if (playerLogic.isBoomTime)
            Remove_EnemyBullet();
    }

    public void Remove_Item()
    {
        GameObject[] itemCoin = objectManager.GetPool("ItemCoin");
        GameObject[] itemPower = objectManager.GetPool("ItemPower");
        GameObject[] itemBoom = objectManager.GetPool("ItemBoom");

        for (int i = 0; i < itemCoin.Length; i++)
        {
            if (itemCoin[i].activeSelf)
                itemCoin[i].SetActive(false);
        }
        for (int i = 0; i < itemPower.Length; i++)
        {
            if (itemPower[i].activeSelf)
                itemPower[i].SetActive(false);
        }
        for (int i = 0; i < itemBoom.Length; i++)
        {
            if (itemBoom[i].activeSelf)
                itemBoom[i].SetActive(false);
        }
    }

    public void StageStart()
    {
        playerLogic.isGameStop = false;

        //#.Stage UI Load
        stageAnim.GetComponent<Text>().text = "STAGE " + stage + "\nSTART";
        clearAnim.GetComponent<Text>().text = "STAGE " + stage + "\nCLEAR!";
        stageAnim.SetTrigger("On");

        //#. Enemy Spawn File Read
        ReadSpawnFile();

        //#. Fade In
        fadeAnim.SetTrigger("In");
    }

    public void StageEnd()
    {
        playerLogic.isGameStop = true;
        playerLogic.PlayerReset();
        bossHpbar.gameObject.SetActive(false);
        bossHpbar.SetHp();

        Remove_Enemy();
        Remove_EnemyBullet();
        Remove_Item();

        //#.Clear UI Load
        clearAnim.SetTrigger("On");

        //#. Fade Out
        fadeAnim.SetTrigger("Out");

        //#. Player Repos
        player.transform.position = playerPos.position;

        //#.Stage Increament
        stage++;
        if (stage > maxStage)
            Invoke("GameClear", 3f);
        else
            Invoke("StageStart", 3f);
    }

    public void GameOver()
    {
        GameOverSet.SetActive(true);
    }

    public void GameClear()
    {
        GameOverSet.transform.GetChild(0).gameObject.SetActive(false);
        GameOverSet.transform.GetChild(1).gameObject.SetActive(true);
        GameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0); // 씬 빌드에 할당된 번호 (씬 이름도 가능)
    }
}
