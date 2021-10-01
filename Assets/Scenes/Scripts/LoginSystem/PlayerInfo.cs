using UnityEngine;

[System.Serializable]
public class PlayerInfo : MonoBehaviour
{
    public int res;
    public string currentCar;
    public string money;
    public static PlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<PlayerInfo>(jsonString);
    }

}
