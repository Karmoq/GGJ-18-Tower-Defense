using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour {

    public static TrainManager S;

    private List<Transform> targets;
    public List<Transform> Targets
    {
        get
        {
            return targets;
        }
    }
        


    void Awake()
    {
        S = this;
        targets = new List<Transform>();
    }

    public void AddTarget(Transform target)
    {
        targets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        targets.Remove(target);
    }

}
