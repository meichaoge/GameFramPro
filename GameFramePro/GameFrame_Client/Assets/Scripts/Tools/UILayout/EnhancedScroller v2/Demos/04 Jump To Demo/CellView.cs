using UnityEngine;
using UnityEngine.UI;
using GameFramePro.UI;

namespace EnhancedScrollerDemos.JumpToDemo
{
    public class CellView : EnhancedScrollerCellView
    {
        public Text cellText;

        public void SetData(Data data)
        {
            cellText.text = data.cellText;
        }
    }
}