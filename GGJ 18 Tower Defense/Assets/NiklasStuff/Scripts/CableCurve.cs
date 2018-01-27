using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class CableCurve : MonoBehaviour
{
    [SerializeField]
    private Transform middlePos;
    [SerializeField]
    private Transform endPos;
    [SerializeField]
    private int divisions = 10;

    private bool locked = false;
    private Vector3 lockedMidPos;
    private Vector3 lockedEndPos;
    [SerializeField]
    private bool runTimeGenerate = false;

    void Start()
    {
        GenerateCurve();
    }

    void Update()
    {
        if(locked)
        {
            middlePos.position = lockedMidPos;
            endPos.position = lockedEndPos;
        }

        if(runTimeGenerate)
            GenerateCurve();
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float oneMinusT = 1.0f - t;
        return oneMinusT * oneMinusT * p0 + 2.0f * oneMinusT * t * p1 + t * t * p2;
    }

    public void GenerateCurve()
    {
        if (middlePos == null || endPos == null)
            return;
        LineRenderer _rend = GetComponent<LineRenderer>();
        _rend.positionCount = (divisions + 1);
        for (int i = 0; i < divisions + 1; i++)
        {
            _rend.SetPosition(i, CalculateBezierPoint((i * 1.0f) / (divisions * 1.0f), transform.position, middlePos.position, endPos.position));
        }
    }

    public void LockPositions(bool _state)
    {
        locked = _state;
        lockedMidPos = middlePos.position;
        lockedEndPos = endPos.position;
    }

    public bool GetLockState()
    {
        return locked;
    }
}
