using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSimpleItem : MonoBehaviour
{
    public Text mText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialed(int index)
    {
        mText.text = "数据" + index;
    }


}
