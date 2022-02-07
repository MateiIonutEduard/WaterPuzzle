using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public Sprite[] colors;
    private Server server;

    public Glass[] tubes;
    public Text label;
    List<Move> moves;

    private Glass src;
    private int len;
    public int level;

    public int count;
    public int extra;

    public void Start()
    {
        moves = new List<Move>();
        server = FindObjectOfType<Server>();
        //SetStage(75);
        level = GetStage();
        label.text = $"Level {level}";
        StartCoroutine("LoadGame");
        count = 5;
    }

    private void SaveMove()
    {
        if(moves.Count == 5) moves.RemoveAt(0);
        var list = new List<int>();

        for(int j = 0; j < len; j++)
        {
            var buf = tubes[j].GetBuffer();

            for (int k = 0; k < buf.Length; k++)
                list.Add(buf[k]);
        }

        var move = new Move(list.ToArray());
        moves.Add(move);
        list.Clear();
    }

    public void GoBack(Text steps)
    {
        if (moves.Count < 2 || count == 0) return;
        DestroyLevel();
        label.text = $"Level {level}";
        var state = moves[moves.Count - 1];

        int[] data = new int[state.len << 2];
        System.Array.Copy(state.buffer, data, data.Length);

        len = data.Length >> 2;

        for (int j = 0; j < len; j++)
            tubes[j].gameObject.SetActive(true);

        int index = 0;
        int size = len - extra;

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                int code = data[index] - 1;
                if(code >= 0) tubes[j].LoadColor(colors[code]);
                index++;
            }
        }

        moves.RemoveAt(moves.Count - 1);
        steps.text = $"{--count}";
    }

    public IEnumerator LoadGame()
    {
        int[] data = server.GetLevel(level);
        len = data.Length >> 2;
        count = 5;

        for (int j = 0; j < len; j++)
            tubes[j].gameObject.SetActive(true);

        extra = Utils.GetExtra(level);
        int size = len - extra;
        int index = 0;

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                tubes[j].LoadColor(colors[data[index] - 1]);
                index++;
            }
        }

        var move = new Move(data);
        moves.Add(move);
        yield return null;
    }

    public void NewGame()
    {
        SetStage(1);
        moves = new List<Move>();
        server = FindObjectOfType<Server>();
        count = 5;

        level = GetStage();
        label.text = $"Level {level}";
        RestartLevel();
    }

    public void GameEnd()
    {
        Application.Quit(0);
    }

    public void AddTube()
    {
        if(len < 18)
            tubes[len++].gameObject.SetActive(true);
    }

    public void RestartLevel()
    {
        DestroyLevel();
        label.text = $"Level {level}";
        int[] data = server.GetLevel(level);
        len = data.Length >> 2;
        count = 5;

        for (int j = 0; j < len; j++)
            tubes[j].gameObject.SetActive(true);

        extra = Utils.GetExtra(level);
        int size = len - extra;
        int index = 0;

        for (int j = 0; j < size; j++)
        {
            for (int i = 0; i < 4; i++)
            {
                tubes[j].LoadColor(colors[data[index] - 1]);
                index++;
            }
        }
    }

    private void DestroyLevel()
    {
        for (int j = 0; j < len; j++)
        {
            tubes[j].Destroy();
            tubes[j].gameObject.SetActive(false);
        }
    }

    public void Update()
    {
        if(GoNext())
        {
            DestroyLevel();
            level++;
            SetStage(level);
            RestartLevel();
        }
    }

    private bool GoNext()
    {
        bool result = true;

        for (int k = 0; k < len; k++)
        {
            if(!tubes[k].IsFull() && !tubes[k].IsEmpty())
            {
                result = false;
                break;
            }
        }

        return result;
    }

    private int GetStage()
    {
        if(!PlayerPrefs.HasKey("stage"))
        {
            PlayerPrefs.SetInt("stage", 1);
            return 1;
        }
        else
        {
            int stage = PlayerPrefs.GetInt("stage");
            return stage;
        }
    }

    private void SetStage(int level)
    {
        if(!PlayerPrefs.HasKey("stage"))
            PlayerPrefs.SetInt("stage", 1);
        else
            PlayerPrefs.SetInt("stage", level);
    }

    public void SetSource(Glass glass)
    {
        if (src == null) src = glass;
        else if (src != glass)
        {
            if (src.UnloadColor(glass))
                SaveMove();
            src = null;
        }
    }
}
