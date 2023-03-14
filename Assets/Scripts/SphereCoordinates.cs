using UnityEngine;

public static class SphereCoordinates {
    
    public static Vector2 UVCoord(CoordinatePoint coordinatePoint) {
        float normX = Mathf.InverseLerp(-Mathf.PI, Mathf.PI, coordinatePoint.longitude); 
        float normY = Mathf.InverseLerp(-Mathf.PI * .5f, Mathf.PI * .5f, coordinatePoint.latitude);

        return new Vector2(normX, normY);
    }
    
    // Calculate latitude and longitude (in radians) from point on unit sphere
    public static CoordinatePoint PointToCoordinate(Vector3 pointOnUnitSphere) {
        float latitude = Mathf.Asin(pointOnUnitSphere.y);
        float longitude = Mathf.Atan2(pointOnUnitSphere.x, -pointOnUnitSphere.z);
        return new CoordinatePoint(latitude, longitude);
    }

    // Calculate point on unit sphere given latitude and longitude (in radians)
    public static Vector3 CoordinateToPoint(CoordinatePoint coordinatePoint) {
        float y = Mathf.Sin(coordinatePoint.latitude * Mathf.Deg2Rad);
        float r = Mathf.Cos(coordinatePoint.latitude * Mathf.Deg2Rad);
        float x = Mathf.Sin(coordinatePoint.longitude * Mathf.Deg2Rad) * r;
        float z = -Mathf.Cos(coordinatePoint.longitude * Mathf.Deg2Rad) * r;
        return new Vector3(x, y, z);
    }    
    
    // Calculate point on unit sphere given latitude and longitude (in radians)
    public static Vector3 CoordinateToPoint(float latitude, float longitude) {
        float latRad = latitude * Mathf.Deg2Rad;
        float lngRad = longitude * Mathf.Deg2Rad;
        
        float y = Mathf.Sin(latRad);
        float r = Mathf.Cos(latRad);
        float x = Mathf.Sin(lngRad) * r;
        float z = -Mathf.Cos(lngRad) * r;
        return new Vector3(x, y, z);
    }
    
    //public static Vector3 PointOnCubeToPointOnSphere(Vector3 p) => Vector3.Normalize(p);
    public static Vector3 PointOnCubeToPointOnSphere(Vector3 p) {
        float x2 = p.x * p.x;
        float y2 = p.y * p.y;
        float z2 = p.z * p.z;

        float x = p.x * Mathf.Sqrt(1 - (y2 + z2) / 2 + (y2 * z2) / 3);
        float y = p.y * Mathf.Sqrt(1 - (z2 + x2) / 2 + (z2 * x2) / 3);
        float z = p.z * Mathf.Sqrt(1 - (x2 + y2) / 2 + (x2 * y2) / 3);
        return new Vector3(x, y, z);
    }
}

public struct CoordinatePoint {
    public CoordinatePoint(float latitude, float longitude) {
        this.longitude = longitude;
        this.latitude = latitude;
    }

    public float longitude;
    public float latitude;
}
