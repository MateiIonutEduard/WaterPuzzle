using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterPuzzle
{
    static int[] buffer;
    static int len, extra;
    static int level;

    static Node init()
    {
        Node root = new Node();
        root.len = len + extra;
        root.buffer = new int[root.len << 2];
        System.Array.Copy(buffer, root.buffer, buffer.Length);
        root.depth = 0;
        return root;
    }

    static void Copy(Node src, Node dest)
    {
        dest.buffer = new int[src.len << 2];
        System.Array.Copy(src.buffer, dest.buffer, src.buffer.Length);
        dest.len = src.len;
    }

    static bool IsSolved(Node root)
    {
        int len = root.len;
        int size = len << 2;
        var buffer = root.buffer;
        int count = 0;

        for (int i = 0; i < len; i++)
        {
            bool ok = true;
            int val = buffer[4 * i];

            for (int j = 1; j < 4; j++)
            {
                int next = buffer[4 * i + j];

                if (val != next)
                {
                    ok = false;
                    break;
                }
            }

            if (ok) count++;
        }

        return count == len;
    }

    static Node[] GetChilds(Node root)
    {
        int len = root.len;
        var list = new List<Node>();

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                if (i == j) continue;
                var child = new Node();

                Copy(root, child);
                child.depth = root.depth + 1;

                if (child.SetState(i, j))
                    list.Add(child);
            }
        }

        return list.ToArray();
    }

    static bool Exists(List<Node> list, Node node)
    {
        for (int i = 0; i < list.Count; i++)
            if (node == list[i]) return true;

        return false;
    }


    static bool Check(ref int depth)
    {
        Node root = init();
        var queue = new PriorityQueue<Node>();
        var list = new List<Node>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            root = queue.Dequeue();
            list.Add(root);

            if (IsSolved(root))
            {
                depth = root.depth;
                return true;
            }

            var childs = GetChilds(root);

            for (int i = 0; i < childs.Length; i++)
            {
                if (IsSolved(childs[i]))
                {
                    depth = childs[i].depth;
                    return true;
                }

                if (!Exists(list, childs[i]))
                    queue.Enqueue(childs[i]);
            }
        }

        return false;
    }

    public static int[] GenPuzzle(int len, int extra, ref int depth, int stage = 1)
    {
        level = stage;
        SetFilter(len, extra);
        Fill();

        while (!Check(ref depth))
            Fill();

        return (int[])buffer.Clone();
    }

    static void Fill()
    {
        int index = 0;
        int n = len << 2;

        int[] color = new int[len];
        int[] code = new int[len];

        for (int i = 0; i < len; i++)
        {
            code[i] = i + 1;
            color[i] = 4;
        }

        if (level > 3) Shuffle(code, len);

        while (n > 0)
        {
            int val = Random.Range(0, 4 * len);
            int glassColor = val >> 2;

            if (color[glassColor] != 0)
            {
                buffer[index++] = code[glassColor];
                color[glassColor]--;
                n--;
            }
        }
    }

    static void Shuffle(int[] color, int len)
    {
        int size = len;
        bool[] all = new bool[16];
        int index = 0;

        while (size-- > 0)
        {
            int code;
            do
                code = Random.Range(1, 16);
            while (all[code]);

            color[index++] = code;
            all[code] = true;
        }
    }

    static void SetFilter(int glasses, int bonus)
    {
        len = glasses;
        extra = bonus;
        int newSize = (len + extra) << 2;
        buffer = new int[newSize];
    }
}