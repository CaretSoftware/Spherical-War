using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParseCityLights : MonoBehaviour {
    public TextAsset cityLightsFile;
    //public PointOnSphere[] cities;
    public GameObject prefab;
    [SerializeField] private float heightMagnitude = 1f;
    
    private void Start() {
        CityLightGroup[] groups = LoadFromFile(cityLightsFile);
        List<CityLight> lightsList = new List<CityLight>();
        Debug.Log(lightsList.Count);
    }
    
    public static CityLightGroup[] LoadFromFile(TextAsset loadFile)
    {
        return ArrayFromJson<CityLightGroup>(loadFile.text);
    }

    public static T[] ArrayFromJson<T>(string jsonString)
    {
        return JsonUtility.FromJson<Holder<T>>(jsonString).items;
    }

    public static string ArrayToJson<T>(T[] array)
    {
        var holder = new Holder<T>(array);
        return JsonUtility.ToJson(holder);
    }
    
    [System.Serializable]
    public struct Holder<T>
    {
        public T[] items;

        public Holder(T[] items)
        {
            this.items = items;
        }
    }
}

public struct CityLightGroup {
    public Bounds bounds;
    public CityLight[] cityLights;
}


[System.Serializable]
public struct CityLight
{
    public Vector3 pointOnSphere;
    public float height;
    public float intensity;
    public float randomT;
}
