using Cysharp.Threading.Tasks;
using PCS.Common;
using PCS.SaveData;
using PCS.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class ScenarioTest : MonoBehaviour
{
    [SerializeField] private Button _execute;
    [SerializeField] private TextMeshProUGUI _executeText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private ScenarioSystem _scenarioSystem;

    // Start is called before the first frame update
    async void Start()
    {
        _execute.interactable = false;
        _executeText.SetText("초기화 작업 진행중");
        await LanguageManager.InitializeAsync();
        await SoundManager.Instance.InitializeAsync();
        await ScenarioSystemManager.Instance.InitializeAsync();

        _execute.onClick.AddListener(ExecuteScenario);
        _executeText.SetText("실행");
        _execute.interactable = true;
    }

    private void ExecuteScenario()
    {
        string path = _inputField.text;
        int selectedIndex = _dropdown.value;
        string language = _dropdown.options[selectedIndex].text;
        OptionSaveData.Instance.Language = language;

        ScenarioSystem system = ScenarioSystemManager.Instance.CreateScenarioSystemBuilder()
            .WithDebugMode()
            .WithEndEvent(async (sys) =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1));
                Destroy(sys.gameObject);
            }).Build();

        system?.StartScenario().Forget();
                                    
    }

}
