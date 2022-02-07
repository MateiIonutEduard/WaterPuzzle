using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Glass : MonoBehaviour
{
    private Image[] childs;
    private int[] buffer;
    private int index;

    public void Awake()
    {
        childs = new Image[4];
        buffer = new int[4];
        index = 0;

        for (int i = 0; i < 4; i++)
        {
            var child = transform.GetChild(i).gameObject;
            childs[i] = child.GetComponent<Image>();
        }
    }

    public int[] GetBuffer()
    { return (int[])buffer.Clone(); }

    public void Destroy()
    {
        childs = new Image[4];
        buffer = new int[4];
        index = 0;

        for (int i = 0; i < 4; i++)
        {
            var child = transform.GetChild(i).gameObject;
            childs[i] = child.GetComponent<Image>();
            var color = childs[i].color;
            color.a = 0f;
            childs[i].color = color;
        }
    }

    public bool IsFull()
    {
        int result = 1;
        int color = buffer[0];

        for (int k = 1; k < 4; k++)
            result += buffer[k] == color ? 1 : 0;

        return result == 4;
    }

    public bool IsEmpty()
    {
        for (int i = 0; i < 4; i++)
            if (buffer[i] != 0)
                return false;

        return true;
    }

    public bool LoadColor(Sprite color)
    {
        if(index == 4) return false;
        int id = int.Parse(color.name.Substring(6));

        buffer[index++] = id;

        childs[index - 1].sprite = color;
        var newColor = childs[index - 1].color;

        newColor.a = 1f;
        childs[index - 1].color = newColor;
        return true;
    }
    
    public bool UnloadColor(Glass glass)
    {
        int j = FindIndex();
        int k = glass.FindIndex();
        if (j == -1) return false;

        if(k >= 0)
        {
            // get color
            int color = glass.buffer[k];
            if (color != buffer[j]) return false;

            int v = FindLastColor(j);
            int t = k - 1;

            if (v == -1) Reshape(this, j, glass, 0);
            else
            {
                for (int r = j; r <= v && t >= 0; r++)
                    Reshape(this, r, glass, t--);
            }
        }
        else
        {
            // empty glass...
            int u = FindLastColor(j);
            k = 3;

            for(int r = j; r <= u; r++)
                Reshape(this, r, glass, k--);
        }

        return true;
    }

    private void Reshape(Glass src, int src_index, Glass dest, int dest_index)
    {
        if (src_index == 4) return;
        int id = int.Parse(childs[src_index].name.Substring(6));
        var newColor = src.childs[src_index].color;

        dest.buffer[dest_index] = src.buffer[src_index];
        src.buffer[src_index] = 0;

        dest.childs[dest_index].sprite = src.childs[src_index].sprite;
        var color = dest.childs[dest_index].color;

        color.a = 1f;
        dest.childs[dest_index].color = color;

        newColor.a = 0f;
        src.childs[src_index].color = newColor;
    }

    private int FindLastColor(int j)
    {
        bool ok = true;
        int color = buffer[j];
        int k = 0;

        for (k = j + 1; k < 4; k++)
            if (color != buffer[k])
            {
                ok = false;
                break;
            }

        if (!ok && k > 0) k--;
        return k;
    }

    private int FindIndex()
    {
        int j;
        bool ok = false;

        for (j = 0; j < 4; j++)
        {
            if (buffer[j] != 0)
            {
                ok = true;
                break;
            }
        }

        return ok ? j : -1;
    }
}
