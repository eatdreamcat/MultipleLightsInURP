using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class RandomLights : MonoBehaviour
{
    
    public int frames = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (frames <= 0) return;
        
        if (Time.frameCount % frames == 0)
        {
            var lights = GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.color = new Color(
                    Random.value,
                    Random.value,
                    Random.value,
                    1.0f
                );
            }
        }
        

    }
}
