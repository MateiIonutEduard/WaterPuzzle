using System;
using System.IO;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    IDbConnection db;

    public void Start()
    {
        db = GetDatabase("puzzle.db");
    }

    private IDbConnection GetDatabase(string name)
    {
#if UNITY_EDITOR
        var dbPath = string.Format(@"Assets/StreamingAssets/{0}", name);
#else
        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, name);

        if (!File.Exists(filepath))
        {
#if UNITY_ANDROID
            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + name);
            while (!loadDb.isDone) ;
            File.WriteAllBytes(filepath, loadDb.bytes);
#elif UNITY_IOS
                 var loadDb = Application.dataPath + "/Raw/" + name;
                File.Copy(loadDb, filepath);
#elif UNITY_WP8
                var loadDb = Application.dataPath + "/StreamingAssets/" + name;
                File.Copy(loadDb, filepath);

#elif UNITY_WINRT
        var loadDb = Application.dataPath + "/StreamingAssets/" + name;
        File.Copy(loadDb, filepath);
#else
    var loadDb = Application.dataPath + "/StreamingAssets/" + name;
    File.Copy(loadDb, filepath);
#endif
        }

        var dbPath = filepath;
#endif
        var connect = new SqliteConnection($"URI=file:{dbPath}");
        return connect;
    }

    public int[] GetLevel(int level)
    {
        db.Open();
        IDbCommand cmd = db.CreateCommand();
        string query = $"select buffer from Level where stage={level}";

        cmd.CommandText = query;
        var reader = cmd.ExecuteReader();
        int[] buffer;

        if(reader.Read())
        {
            var list = (byte[])reader["buffer"];
            buffer = ReadLevel(list);
        }
        else
        {
            int depth = 0;
            int glasses, extra;
            Utils.Fs(level, out glasses, out extra);
            buffer = WaterPuzzle.GenPuzzle(glasses, extra, ref depth, level);
            var list = WriteLevel(buffer);

            query = $"insert into Level(buffer, stage, difficulty) values(@data, {level}, {depth});";
            cmd = db.CreateCommand();
            cmd.CommandText = query;

            var data = new SqliteParameter("data", list);
            cmd.Parameters.Add(data);
            cmd.ExecuteNonQuery();
        }

        db.Close();
        return buffer;
    }

    private int[] ReadLevel(byte[] buffer)
    {
        var list = new List<int>();

        for(int i = 0; i < buffer.Length; i++)
        {
            int val = (buffer[i] >> 4) & 0xF;
            list.Add(val);
            list.Add(buffer[i] & 0xF);
        }

        return list.ToArray();
    }

    private byte[] WriteLevel(int[] buffer)
    {
        var list = new List<byte>();

        for (int i = 0; i < buffer.Length; i += 2)
        {
            byte val = (byte)((buffer[i] << 4) | buffer[i + 1]);
            list.Add(val);
        }

        return list.ToArray();
    }
}
