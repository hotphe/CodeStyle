using System.Collections.Generic;
using UnityEngine;
using PCS.Common;

public class ScenarioChoiceSO : ScriptableObject
{
    public SerializedDictionary<int, List<ScenarioChoice>> Data = new SerializedDictionary<int, List<ScenarioChoice>>();
}
