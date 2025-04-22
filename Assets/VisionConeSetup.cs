using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeSetup : MonoBehaviour {
    public PolygonCollider2D coll;
    public float baseRadius;
    public float coneRadius;
    public float angle;
    public int arcResolution;
    public float adjustment;

    public bool update; 
    public int updateFrequency;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        coll.SetPath(0, CalculatePoints());
    }

    public Vector2[] CalculatePoints() {
        Vector2[] points = new Vector2[arcResolution+2];
        float baseX = -baseRadius/Mathf.Sqrt(1/Mathf.Pow(Mathf.Tan(angle/2)+adjustment, 2) + 1);
        float baseY = Mathf.Sqrt(1 - Mathf.Pow(baseX, 2));
        points[0] = new Vector2(baseX, baseY);

        //Half of arcResolution, rounded UP, minus one.
        int halfArcResMinusOne = (arcResolution&1)==0 ? arcResolution/2 : arcResolution/2+1;

        for (int i = 0; i < halfArcResMinusOne; i++) {
            float theta = angle*((halfArcResMinusOne-i-1)/(arcResolution - 1f));
            points[i+1] = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta))*coneRadius;
        }

        //Mirror all the points for the remainder.

        //Half of points.Length, rounded DOWN, minus one.
        int halfPointsMinusOne = points.Length/2;

        for (int i = 0; i < halfPointsMinusOne; i++) {
            points[points.Length-i-1] = new Vector2(points[i].x, -points[i].y);
        }
        return points;
    }

	private void Update() {
        if (update) {
            if (++i == updateFrequency) {
                i = 0;
                coll.SetPath(0, CalculatePoints());
            }
        }
    }
}
