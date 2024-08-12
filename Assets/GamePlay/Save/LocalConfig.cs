using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LocalConfig
{
    public static Configdata cacheConfigData;
    public class Configdata
    {
        public float soundVolume;
        public float musicVolume;
        public int FPS;
        public bool isFullScreen;
        public int resolution;
        public Configdata(float soundVolume, float musicVolume, int fPS, bool isFullScreen, int resolution)
        {
            this.soundVolume = soundVolume;
            this.musicVolume = musicVolume;
            FPS = fPS;
            this.isFullScreen = isFullScreen;
            this.resolution = resolution;
        }
    }
    public static void SaveConfigData(Configdata configdata)
    {
        if(!File.Exists(Application.persistentDataPath))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath);
        }
        string jsonData=JsonConvert.SerializeObject(configdata);
        File.WriteAllText(Application.persistentDataPath + string.Format("/config.json"), jsonData);
        cacheConfigData = configdata;
    }
    public static void InitConfigDate()
    {
        if (!File.Exists(Application.persistentDataPath))
        {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath);
        }
        string path = Application.persistentDataPath + "/config.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
    public static Configdata LoadConfigData()
    {
        string path = Application.persistentDataPath + "/config.json";
        if (File.Exists(path))
        {
            if(cacheConfigData!=null) return cacheConfigData;
            else
            {
                string jsonData = File.ReadAllText(path);
                Configdata configdata = JsonConvert.DeserializeObject<Configdata>(jsonData);
                cacheConfigData = configdata;
                return configdata;
            }
            
        }
        else
        {
            return null;
        }
    }
}
