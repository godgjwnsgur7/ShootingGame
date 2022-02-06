using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
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

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;


    void Awake()
    {
        spawnList = new List<Spawn>();
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" };
        nextSpawnDelay = 2f;
        ReadSpawnFile();
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

        // #2.Read Respawn File
        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        // #3.Save Data
        while(stringReader != null)
        {
            string line = stringReader.ReadLine();
            Debug.Log(line);

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
        }

        int enemyPoint = spawnList[spawnIndex].point;
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);
        enemy.transform.position = spawnPoints[enemyPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; // 적 생성후 플레이어 변수를 넘겨줌
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

    public void GameOver()
    {
        GameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0); // 씬 빌드에 할당된 번호 (씬 이름도 가능)
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
}
