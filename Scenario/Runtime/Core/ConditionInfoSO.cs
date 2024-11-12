using System.Collections.Generic;
using UnityEngine;
using PCS.Common;

public class ConditionInfoSO : ScriptableObject
{
    public SerializedDictionary<int, List<ConditionInfo>> Data = new SerializedDictionary<int, List<ConditionInfo>>();
}
