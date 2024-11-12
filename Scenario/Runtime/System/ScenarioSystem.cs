using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using PCS.Sound;

public class ScenarioSystem : MonoBehaviour
{
    [SerializeField] private ScenarioBackgroundView _backgroundView;
    [SerializeField] private ScenarioDialogueView _dialogueView;
    [SerializeField] private ScenarioCharacterView _characterView;
    [SerializeField] private ScenarioChoiceView _choiceView;
    [SerializeField] private ScenarioNextButtonView _nextButtonView;

    private Dictionary<int, List<ScenarioScript>> _scenarioScripts;
    private Dictionary<int, List<ScenarioChoice>> _scenarioChoices;
    private Dictionary<int, List<ScenarioTrigger>> _scenarioTriggers;

    private event Func<UniTask> _onScenarioStarted;
    private event Func<UniTask> _onScenarioEnded;

    private ScenarioScript _currentScenarioScript;
    private bool _canNext = false;

    public void Initialize(Dictionary<int, List<ScenarioScript>> scenarioScripts,
        Dictionary<int, List<ScenarioChoice>> scenarioChoices, 
        Dictionary<int, List<ScenarioTrigger>> scenarioTriggers, 
        Func<UniTask> startEvent, 
        Func<UniTask> endEvent)
    {
        _scenarioScripts = scenarioScripts;
        _scenarioChoices = scenarioChoices;
        _scenarioTriggers = scenarioTriggers;
        _onScenarioStarted += startEvent;
        _onScenarioEnded += endEvent;
        InitializeViews();
    }

    private void InitializeViews()
    {
        _nextButtonView.Initialize(OnClickNext);
        _dialogueView.Initialize();
    }

    public async UniTask StartScenario()
    {
        if(_onScenarioStarted != null)
            await _onScenarioStarted();

        gameObject.SetActive(true);
        ShowDialogue(_scenarioScripts.ElementAt(0).Value.First()).Forget();
    }

    private void OnClickNext()
    {
        if (_canNext)
        {
            GetNext();
        }else
        {
            _dialogueView.SkipTypingAnimation();
        }
    }
    private void GetNext()
    {
        ENextType type = _currentScenarioScript.NextType;
        int id = _currentScenarioScript.TypeID;
        
        switch(type)
        {
            case ENextType.End:
                EndScenario();
                break;
            case ENextType.Script:
                ShowDialogue(_scenarioScripts[id].First()).Forget();
                break;
            case ENextType.Choice:
                ShowChoice(_scenarioChoices[id]).Forget();
                break;
            case ENextType.Trigger:
                CheckTrigger(_scenarioTriggers[id]);
                break;
        }
    }

    private void EndScenario()
    {
        _nextButtonView.DisableButton();
        if (_onScenarioEnded != null)
            _onScenarioEnded().Forget();
    }

    private async UniTask ShowDialogue(ScenarioScript script)
    {
        _currentScenarioScript = script;
        
        _backgroundView.ChangeSprite(_currentScenarioScript.ImageBackground);
        _characterView.Show(_currentScenarioScript.ImageList);
        ChangeBGM(script.BGM);

        _canNext = false;
        _canNext = await _dialogueView.Show(script);
    }

    private async UniTask ShowChoice(List<ScenarioChoice> choices)
    {
        _backgroundView.ChangeSprite(_currentScenarioScript.ImageBackground);

        var selectables = choices.Where(x => x.Condition == -1 || CheckCondition(x.Condition)).ToList();
        int id = await _choiceView.ShowAsync(selectables);
        _choiceView.Hide();
        ShowDialogue(_scenarioScripts[id].First()).Forget();
    }

    private void CheckTrigger(List<ScenarioTrigger> triggers)
    {
        foreach(ScenarioTrigger trigger in triggers) 
        { 
            if(CheckCondition(trigger.Condition) || trigger.Condition == -1)
            {
                ShowDialogue(_scenarioScripts[trigger.ScriptID].First()).Forget();
                break;
            }
        }
    }
    
    private void ChangeBGM(string bgm)
    {
        SoundManager.Instance.GetBGMBuilder().PlayBGM(bgm);
    }

    private bool CheckCondition(int conditionId)
    {
        return ConditionSaveData.Instance.ContainCondition(conditionId);
    }

}
