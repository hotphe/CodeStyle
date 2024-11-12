using System.Collections.Generic;
using UnityEngine;
using PCS.Common;
[CreateAssetMenu]
public class ScenarioScriptSO : ScriptableObject
{
    public SerializedDictionary<int,List<ScenarioScript>> Data = new SerializedDictionary<int,List<ScenarioScript>>();
}
