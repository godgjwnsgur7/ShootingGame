using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    int bulletSpeed;
    public float maxShotDelay;
    public float curShotDelay;

    public ObjectManager objectManager;

    public Vector3 followPos;
    public int followDelay;
    public Transform Parent;
    public Queue<Vector3> parentPos;


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
        else if (parentPos.Count > followDelay)
            followPos = Parent.position;
    }

    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        if (!Input.GetButton("Fire1") || curShotDelay < maxShotDelay)
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
