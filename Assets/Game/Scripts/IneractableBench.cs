using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IneractableBench : InteractableObject
{
    public override string Name_t { get { return "Rest"; } }
    public override void OnEnter()
    {
        
    }
    public override void OnExit()
    {

    }
    public override void OnActivate()
    {
        Debug.Log("works");
    }
}
