using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using Unity.VisualScripting.FullSerializer;

public class FurnitureDatabase : MonoBehaviour
{
    private const string databaseURL = "https://unistdfurnituredatabase-default-rtdb.firebaseio.com/";
    void Start()
    {
       
    }

    public static void putFurniture(FurnitureInfo furniture, string furnitureId)
    {
        RestClient.Put<FurnitureInfo>($"{databaseURL}/furnitures/{furnitureId}.json", furniture);
    }

    public delegate void getFurnitureCallback(FurnitureInfo user);
 
    //가구 정보 가져오기
    public static void getFurniture(string furnitureId, getFurnitureCallback callback)
    {
        RestClient.Get<FurnitureInfo>($"{databaseURL}/furnitures/{furnitureId}.json").Then(furniture =>
        {
            callback(furniture);
        });
    }

    public delegate void getADCallback(FurnitureAD user);

    //가구 광고 가져오기
    public static void getAD(string adId, getADCallback callback)
    {
        RestClient.Get<FurnitureAD>($"{databaseURL}/AD/{adId}.json").Then(ad =>
        {
            callback(ad);
        });
    }

    public delegate void getFurnituresCallback(Dictionary<string, FurnitureInfo> users);
    private static fsSerializer serializer = new fsSerializer();

    public static void getFurnitures(getFurnituresCallback callback)
    {
        RestClient.Get($"{databaseURL}users.json").Then(response =>
        {
            var responseJson = response.Text;
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, FurnitureInfo>), ref deserialized);
            var furnitures = deserialized as Dictionary<string, FurnitureInfo>;
            callback(furnitures);
        });
    }
}
