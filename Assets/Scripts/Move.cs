using System;
using System.Collections;
using System.Collections.Generic;

public class Move
{
    public int[] buffer;
    public int len;

    public Move(int[] buffer)
    {
        this.buffer = new int[buffer.Length];
        Array.Copy(buffer, this.buffer, buffer.Length);
        len = buffer.Length >> 2;
    }

    public int[] GetData()
    { return (int[])buffer.Clone(); }
}
