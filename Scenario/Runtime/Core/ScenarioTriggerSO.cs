using System.Collections.Generic;
using UnityEngine;
using PCS.Common;

public class ScenarioTriggerSO : ScriptableObject
{
    public SerializedDictionary<int, List<ScenarioTrigger>> Data = new SerializedDictionary<int, List<ScenarioTrigger>>();
}
