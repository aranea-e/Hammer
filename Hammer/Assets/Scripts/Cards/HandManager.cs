using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    public HandCard[] cards;
    public float speed;
    public Vector3 targetUp;
    public Vector3 targetDown;

    public void Update()
    {
        for (int i = 0; i < 21; i++)
        {
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x <= cards[i].trans.position.x + 1.0f/6.0f && Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= cards[i].trans.position.x - .5 && Camera.main.ScreenToWorldPoint(Input.mousePosition).y <= -3)
            {
                var step = speed * Time.deltaTime;
                targetUp.x = cards[i].trans.position.x;
                targetUp.z = cards[i].trans.position.z;
                cards[i].trans.position = Vector3.MoveTowards(cards[i].trans.position, targetUp, step);
            } else
            {
                var step = speed * Time.deltaTime;
                targetDown.x = cards[i].trans.position.x;
                targetDown.z = cards[i].trans.position.z;
                cards[i].trans.position = Vector3.MoveTowards(cards[i].trans.position, targetDown, step);
            }
        }
    }
}
