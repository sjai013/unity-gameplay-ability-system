using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class GunnerLightning : MonoBehaviour
    {

        LineRenderer LR;

        //public Transform start;
        public Transform end;
        Transform[] branch;

        public float updateInterval = 0.5f;
        float updateTime = 0;

        public int pointCount = 10;
        public float randomOffset = 0.5f;
        Vector3[] points;

        // Use this for initialization
        void Start()
        {
            LR = GetComponent<LineRenderer>();
            points = new Vector3[pointCount];
            LR.positionCount = pointCount;
            LR.useWorldSpace = false;
        }

        void Update()
        {


            if (Time.time >= updateTime)
            {
                LR.positionCount = pointCount;

                points[0] = Vector3.zero;
                Vector3 Segment = (end.position - transform.position) / (pointCount - 1);

                for (int i = 1; i < pointCount - 1; i++)
                {
                    points[i] = Segment * i;
                    points[i].y += Random.Range(-randomOffset, randomOffset);
                    points[i].x += Random.Range(-randomOffset, randomOffset);
                }

                points[pointCount - 1] = (end.position - transform.position);
                LR.SetPositions(points);

                updateTime += updateInterval;
            }


        }
    }

}