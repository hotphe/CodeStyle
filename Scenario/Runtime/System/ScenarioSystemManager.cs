using UnityEngine;
using PCS.Common;
using Cysharp.Threading.Tasks;

public class ScenarioSystemManager : MonoSingleton<ScenarioSystemManager>
{
    private ScenarioSystem _scenarioSystemPrefab;

    public async UniTask InitializeAsync()
    {
        DontDestroyOnLoad(gameObject);
        GameObject obj = (GameObject) await Resources.LoadAsync("ScenarioSystem");
        if(obj == null)
        {
            Debug.LogError("Failed to load ScenarioSystem prefab.");
            return;
        }

        _scenarioSystemPrefab =  obj.GetComponent<ScenarioSystem>();

        if(_scenarioSystemPrefab == null)
        {
            Debug.LogError($"There is no {typeof(ScenarioSystem).Name} component in the loaded prefab.");
            return;
        }
    }
    public ScenarioSystemBuilder CreateScenarioSystemBuilder()
    {
        if (_scenarioSystemPrefab == null)
        {
            Debug.LogError("ScenarioSystemManager is not initialized. Call InitializeAsync first.");
            return null;
        }
        return new ScenarioSystemBuilder(_scenarioSystemPrefab);
    }
}
