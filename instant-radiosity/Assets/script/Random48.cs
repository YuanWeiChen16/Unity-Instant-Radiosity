﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Random48
{
    static long seed = 1;
    public static float Get()
    {
        seed = (0x5DEECE66DL * seed + 0xB16) & 0xFFFFFFFFFFFFL;
        return (seed >> 16) / (float)0x100000000L;
    }
}