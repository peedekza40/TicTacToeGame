using SqlCipher4Unity3D;
using UnityEngine;

#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif

namespace Infrastructure.Entities
{
    public class DbContextBuilder
    {
        public SQLiteConnection Connection;
        private string DatabaseName = "TicTacToe.db";

        public DbContextBuilder()
        {
            #if UNITY_EDITOR
                var dbPath = Application.streamingAssetsPath + "/" + DatabaseName;
            #else
                // check if file exists in Application.persistentDataPath
                string filepath = string.Format("{0}/{1}", Application.persistentDataPath, DatabaseName);

                if (!File.Exists(filepath))
                {
                    Debug.Log("Database not in Persistent path");
                    // if it doesn't ->
                    // open StreamingAssets directory and load the db ->

                    #if UNITY_ANDROID
                        WWW loadDb = new WWW ("jar:file://" + Application.dataPath + "!/assets/" + DatabaseName); // this is the path to your StreamingAssets in android
                        while (!loadDb.isDone) { } // CAREFUL here, for safety reasons you shouldn't let this while loop unattended, place a timer and error check
                        Debug.Log(loadDb.isDone);
                        // then save to Application.persistentDataPath
                        File.WriteAllBytes (filepath, loadDb.bytes);
                    #elif UNITY_IOS
                        string loadDb = Application.dataPath + "/Raw/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                        // then save to Application.persistentDataPath
                        File.Copy (loadDb, filepath);
                    #elif UNITY_WP8
                        string loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                        // then save to Application.persistentDataPath
                        File.Copy (loadDb, filepath);
                        
                    #elif UNITY_WINRT
                        string loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                        // then save to Application.persistentDataPath
                        File.Copy (loadDb, filepath);
                    #elif UNITY_STANDALONE_OSX
                        string loadDb = Application.dataPath + "/Resources/Data/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                        // then save to Application.persistentDataPath
                        File.Copy(loadDb, filepath);
                    #else
                        string loadDb = Application.dataPath + "/StreamingAssets/" + DatabaseName; // this is the path to your StreamingAssets in iOS
                        // then save to Application.persistentDataPath
                        File.Copy(loadDb, filepath);
                    #endif
                        Debug.Log("Database written");
                }
                var dbPath = filepath;
            #endif  
            Connection = new SQLiteConnection(dbPath, "Asdf+1234");
        }
        
    }
}

