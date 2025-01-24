using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;

public class ScenarioSystemBuilder
{
    private readonly ScenarioSystem _scenarioSystemPrefab;
    private string _path;
    private int _key;
    private bool _isDebug;
    private Dictionary<int, List<ScenarioScript>> _scenarioScripts = new();
    private Dictionary<int, List<ScenarioChoice>> _scenarioChoices = new();
    private Dictionary<int, List<ScenarioTrigger>> _scenarioTriggers = new();
    private event Func<ScenarioSystem, UniTask> _onEndedEvent;
    private event Func<ScenarioSystem, UniTask> _onStartedEvent;

    public ScenarioSystemBuilder(ScenarioSystem scenarioSystemPrefab)
    {
        _scenarioSystemPrefab = scenarioSystemPrefab ??
            throw new ArgumentNullException(nameof(scenarioSystemPrefab));
    }

    public ScenarioSystemBuilder WithScenarioKey(int key) 
    {
        _key = key;
        return this;
    }

    public ScenarioSystemBuilder WithDebugMode()
    {
        _isDebug = true;
        return this;
    }

    public ScenarioSystemBuilder WithEndEvent(Func<ScenarioSystem, UniTask> endedEvent)
    {
        _onEndedEvent = endedEvent ??
            throw new ArgumentNullException(nameof(endedEvent));
        return this;
    }

    public ScenarioSystemBuilder WithStartEvent(Func<ScenarioSystem, UniTask> startedEvent)
    {
        _onStartedEvent = startedEvent ??
            throw new ArgumentNullException(nameof(startedEvent));
        return this;
    }

    public ScenarioSystem Build()
    {
        if (_scenarioSystemPrefab == null)
        {
            Debug.LogError("ScenarioSystem prefab is null.");
            return null;
        }

        if (!_isDebug && string.IsNullOrEmpty(_path))
        {
            Debug.LogError("Path is required when not in debug mode.");
            return null;
        }

        if (_isDebug)
        {
            if (!LoadDebugData())
                return null;
        }
        else
        {
            if (!LoadScenarioData())
                return null;
        }

        ScenarioSystem scenarioSystem = GameObject.Instantiate(_scenarioSystemPrefab);

        Func<UniTask> wrappedStartingEvent = null;
        Func<UniTask> wrappedEndedEvent = null;

        if (_onStartedEvent != null)
            wrappedStartingEvent = async () => await _onStartedEvent(scenarioSystem);
        if (_onEndedEvent != null)
            wrappedEndedEvent = async () => await _onEndedEvent(scenarioSystem);

        scenarioSystem.Initialize(_scenarioScripts, _scenarioChoices, _scenarioTriggers,
            wrappedStartingEvent, wrappedEndedEvent);

        return scenarioSystem;
    }

    private bool LoadDebugData()
    {
        try
        {
            if(ScenarioScriptParserV2.Instance.LoadData())
            {
                var values = ScenarioScriptParserV2.Instance.GetAllInfos();
                foreach(var v in values)
                {
                    if (_scenarioScripts.ContainsKey(v.ScriptID))
                        _scenarioScripts[v.ScriptID].Add(v);
                    else
                        _scenarioScripts[v.ScriptID] = new List<ScenarioScript>() { v };
                }
            }
            if(ScenarioChoiceParserV2.Instance.LoadData())
            {
                var values = ScenarioChoiceParserV2.Instance.GetAllInfos();
                int id;
                foreach (var v in values)
                {
                    id = v.FirstOrDefault().ChoiceID;
                    _scenarioChoices[id] = v;
                }
            }
            if(ScenarioTriggerParserV2.Instance.LoadData())
            {
                var values = ScenarioTriggerParserV2.Instance.GetAllInfos();
                int id;
                foreach (var v in values)
                {
                    id = v.FirstOrDefault().TriggerID;
                    _scenarioTriggers[id] = v;
                }
            }
            return true;
        }catch (Exception ex)
        {
            Debug.LogError($"Error loading data: {ex.Message}");
            return false;
        }
    }

    private bool LoadScenarioData()
    {
        //추후 Key 사용에 따라 수정.
        try
        {
            var scriptSO = Resources.Load<ScenarioScriptSO>($"{_key}/ScenarioScript");
            var choiceSO = Resources.Load<ScenarioChoiceSO>($"{_key}/ScenarioChoice");
            var triggerSO = Resources.Load<ScenarioTriggerSO>($"{_key}/ScenarioTrigger");

            if (scriptSO == null || choiceSO == null || triggerSO == null)
            {
                Debug.LogError($"Failed to load scenario data from path: {_path}");
                return false;
            }

            _scenarioScripts = scriptSO.Data.DeepCopy();
            _scenarioChoices = choiceSO.Data.DeepCopy();
            _scenarioTriggers = triggerSO.Data.DeepCopy();

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading scenario data: {ex.Message}");
            return false;
        }
    }
}