﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollIndexCallback3 : LoopScrollRectItemBase
{
    public Text text;

    public override void InitialedScrollCellItem(int idx)
    {
        base.InitialedScrollCellItem(idx);
        if (text != null)
        {
            text.text = name;
        }
    }
}
