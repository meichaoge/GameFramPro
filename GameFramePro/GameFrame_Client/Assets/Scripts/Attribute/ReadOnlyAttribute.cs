using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>///  Inspector 下显示只读属性/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ReadOnlyAttribute : PropertyAttribute
{
}