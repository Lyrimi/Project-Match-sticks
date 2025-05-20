using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionConeSetup : MonoBehaviour {
    public PolygonCollider2D coll;
    public float baseRadius;
    public float coneRadius;
    public float angle;
    public int arcResolution;
    public int circleResolution;
    //public float adjustment;
    public int iterations;

    public bool update; 
    public int updateFrequency;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        coll.SetPath(0, CalculatePoints());
    }

    public Vector2[] CalculatePoints() {
        Vector2[] points = new Vector2[circleResolution+arcResolution+2];
        float baseX = 0;
        float baseY = 0;
        float slope = angle;
        float coneCornerX = Mathf.Cos(angle/2)*coneRadius;
        float coneCornerY = Mathf.Sin(angle/2)*coneRadius;
        for (int i = 0; i < iterations; i++) {
            if (i > 0) {
                slope = Mathf.Atan((coneCornerY - baseY)/(coneCornerX - baseX))*2;
            }
            baseX = -baseRadius/Mathf.Sqrt(1/Mathf.Pow(Mathf.Tan(slope/2), 2) + 1);
            baseY = Mathf.Sqrt(Mathf.Pow(baseRadius, 2) - Mathf.Pow(baseX, 2));
        }

        //Half of circleResolution, rounded UP.
        int halfCircleRes = (circleResolution&1)==0 ? circleResolution/2 : circleResolution/2+1;

        points[halfCircleRes] = new Vector2(baseX, baseY);

        float horizontallyFlippedYAngle = Mathf.Asin(baseY/baseRadius);
        for (int i = 0; i < halfCircleRes; i++) {
            float theta = Mathf.PI-horizontallyFlippedYAngle*(2*(circleResolution-i)/(circleResolution+1f)-1);
            points[halfCircleRes-i-1] = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta))*baseRadius;
        }

        //Half of arcResolution, rounded UP.
        int halfArcRes = (arcResolution&1)==0 ? arcResolution/2 : arcResolution/2+1;

        for (int i = 0; i < halfArcRes; i++) {
            float theta = angle*(-i/(arcResolution - 1f)+.5f);
            points[i+halfCircleRes+1] = new Vector2(Mathf.Cos(theta), Mathf.Sin(theta))*coneRadius;
        }

        //Mirror all the points for the remainder.

        //Half of points.Length, rounded DOWN.
        int halfPoints = points.Length/2;

        //Jank ((circleResolution&1)==0?0:1) everywhere, but it works, i think.
        for (int i = (circleResolution&1)==0?0:1; i < halfPoints+((circleResolution&1)==0?0:1); i++) {
            points[points.Length-i-1+((circleResolution&1)==0?0:1)] = new Vector2(points[i].x, -points[i].y);
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
