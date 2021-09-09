using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float time {set; get;}
    public float timer;

    public Timer(){
        timer = 0;
        time = 0;
    }
    public Timer(float time){
        this.time = time;
        timer = 0;
    }
    
    public void Start(){
        Debug.Assert(time > 0);
        timer = time;
    }

    public void Zero(){
        timer = 0;
    }

    public static implicit operator bool(Timer timer){
        return timer.timer > 0;
    }

    public void Update()
    {
        timer = Mathf.Max(timer - Time.deltaTime, 0);
    }
}
