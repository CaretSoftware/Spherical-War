using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class CSVReader : MonoBehaviour {

    [SerializeField] private TextAsset textAsset;
    [SerializeField] private GameObject cityPrefab;
    
    [System.Serializable]
    public class City {
        public City(string name, string country, bool capital, int population, float latitude, float longitude, int id) {
            this.name = name;
            this.country = country;
            this.capital = capital;
            this.population = population;
            this.latitude = latitude;
            this.longitude = longitude;
            this.id = id;
        }
        
        public string name;
        public string country;
        public bool capital;
        public int population;
        public float latitude;
        public float longitude;
        public int id;
    }

    [System.Serializable]
    public class CityData {
        public City[] cities;
    }

    public CityData cityData = new CityData();
    
    private void Start() {
        ReadCSV();
    }

    [SerializeField] private float citySizeMin = .05f;
    [SerializeField] private float citySizeMax = .01f;
    [SerializeField] private float cityHeight = 5f;
    
    private void ReadCSV() {
        string[] data = textAsset.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        int columns = 7;
        int rows = data.Length / columns - 1; // - 1 to ignore first row

        cityData.cities = new City[rows];
        
        for (int row = 1; row < rows; row++) {   // i = 1 to skip first row
            
            // 1 = name
            string cityName = data[columns * row]; // Tokyo
            // 2 = lat
            float latitude = float.Parse( data[columns * row + 1]);
            // 3 = lng
            float longitude = float.Parse( data[columns * row + 2]);
            // 4 = Country
            string country = data[columns * row + 3];
            // 5 = capital
            bool capital = data[columns * row + 4].Equals("primary");
            // 6 = pop
            int population = int.Parse(data[columns * row + 5]);
            // 7 = id
            int id = int.Parse(data[columns * row + 6]);
            
            cityData.cities[row - 1] = new City(cityName, country, capital, population, latitude, longitude, id);

            Vector3 point = SphereCoordinates.CoordinateToPoint(latitude, longitude) * cityHeight;

            GameObject go = Instantiate(cityPrefab, point, quaternion.identity, this.transform);

            float size = Mathf.InverseLerp(0f, 39105000f, population);
            go.transform.localScale = Vector3.Lerp( Vector3.one * citySizeMin, Vector3.one * citySizeMax, Ease.EaseOutCubic(size));
        }
    }
}
