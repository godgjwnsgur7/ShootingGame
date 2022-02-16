using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    int bulletSpeed;
    float maxShotDelay;
    float curShotDelay;

    [SerializeField] ObjectManager objectManager;
    [SerializeField] Player player;

     Vector3 followPos;
     int followDelay;
    [SerializeField] Transform Parent;
     Queue<Vector3> parentPos;


    void Awake()
    {
        parentPos = new Queue<Vector3>();
        bulletSpeed = 7;
        maxShotDelay = 2f;
        followDelay = 15; // frame
    }

    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        //#.Input Pos
        if(!parentPos.Contains(Parent.position))
            parentPos.Enqueue(Parent.position);

        //#.Output Pos
        if (parentPos.Count > followDelay)
            followPos = parentPos.Dequeue();
        else if (parentPos.Count < followDelay)
            followPos = Parent.position;
    }

    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (!player.isButtonA || curShotDelay < maxShotDelay)
            return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * bulletSpeed, ForceMode2D.Impulse);
       
        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}
