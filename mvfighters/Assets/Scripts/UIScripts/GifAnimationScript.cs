using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GifAnimationScript : MonoBehaviour
{
    public List<Sprite> gifFrames;
    public Image gifImage;

    public float delay;
    private float lastTime;
    private int currenFrame;
    // Start is called before the first frame update
    void Start()
    {
        currenFrame = 0;
        lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (currenFrame >= gifFrames.Count)
        {
            currenFrame = 0;
        }

        if (!(Time.time - lastTime > delay)) return;
        gifImage.sprite = gifFrames[currenFrame];
        currenFrame++;
        lastTime = Time.time;
    }
}
