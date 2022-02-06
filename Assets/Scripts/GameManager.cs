using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string[] enemyObjs;
    public Transform[] spawnPoints;

    public float maxSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject GameOverSet;
    public ObjectManager objectManager;

    void Awake()
    {
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL" };
        maxSpawnDelay = 2f;    
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            maxSpawnDelay = Random.Range(0.5f, 3f);
            curSpawnDelay = 0;
        }

        //#. UI Score Update
        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);
        GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);
        enemy.transform.position = spawnPoints[ranPoint].position;

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; // 적 생성후 플레이어 변수를 넘겨줌
        enemyLogic.objectManager = objectManager;

        if(ranPoint == 5 || ranPoint == 6) // #.Right Spawn
        {
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
            enemy.transform.Rotate(Vector3.back * 90);
        }
        else if(ranPoint == 7 || ranPoint == 8) // #.Left Spawn
        {
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
            enemy.transform.Rotate(Vector3.forward * 90);
        }
        else // #. Front Spawn
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1));
        }
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
