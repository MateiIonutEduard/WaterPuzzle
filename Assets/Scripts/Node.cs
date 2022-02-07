using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Node : IComparable<Node>
{
    public int[] buffer;
    public int len;
    public int depth;

    public static bool operator ==(Node left, Node right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Node left, Node right)
    {
        return !(left == right);
    }

    public bool SetState(int src, int dest)
    {
        int j = FindIndex(src);
        int k = FindIndex(dest);
        if (j == -1) return false;

        if (k >= 0)
        {
            int color = buffer[4 * dest + k];
            int next = buffer[4 * src + j];

            if (color != next && color > 0 || next == 0) return false;

            int v = FindLastColor(src, j);
            int t = k - 1;

            if (v == -1) Slide(src, j, dest, 0);
            else
            {
                for (int r = j; r <= v && t >= 0; r++)
                    Slide(src, r, dest, t--);
            }
        }
        else
        {
            int u = FindLastColor(src, j);
            k = 3;

            for (int r = j; r <= u; r++)
                Slide(src, r, dest, k--);
        }

        return true;
    }

    public void Slide(int src, int src_index, int dest, int dest_index)
    {
        if (src_index == 4) return;
        int sptr = 4 * src + src_index;
        int dptr = 4 * dest + dest_index;

        buffer[dptr] = buffer[sptr];
        buffer[sptr] = 0;
    }

    public int FindIndex(int i)
    {
        int j;
        bool ok = false;

        for (j = 0; j < 4; j++)
        {
            int index = 4 * i + j;
            int temp = buffer[index];

            if (temp != 0)
            {
                ok = true;
                break;
            }
        }

        return ok ? j : -1;
    }

    public int FindLastColor(int i, int j)
    {
        bool ok = true;
        int color = buffer[4 * i + j];
        int k = 0;

        for (k = j + 1; k < 4; k++)
            if (color != buffer[4 * i + k])
            {
                ok = false;
                break;
            }

        if (!ok && k > 0) k--;
        return k;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (ReferenceEquals(obj, null)) return false;

        Node left = this;
        Node right = (Node)obj;

        if (left.len != right.len) return false;
        int size = left.len << 2;

        for (int i = 0; i < size; i++)
            if (left.buffer[i] != right.buffer[i])
                return false;

        return true;
    }

    private int FindOrder()
    {
        int result = 0;
        int[] color = new int[16];

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < 4; j++)
                color[buffer[4 * i + j]]++;

            int max = 5;

            for (int k = 0; k < 16; k++)
            {
                if (max > color[k] && color[k] != 0)
                    max = color[k];

                color[k] = 0;
            }

            result += (4 - max);
        }

        return result;
    }

    static int Compare(Node x, Node y)
    {
        int left = x.depth + x.FindOrder();
        int right = y.depth + y.FindOrder();
        if (left < right) return -1;
        if (left > right) return 1;
        return 0;
    }

    public int CompareTo(Node other)
    {
        return Compare(this, other);
    }
}
