using System;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioNextButtonView : MonoBehaviour
{
    [SerializeField] private Button _nextButton;

    public void Initialize(Action ClickEvent)
    {
        _nextButton.onClick.AddListener(()=>ClickEvent());
    }

    public void DisableButton()
    {
        _nextButton.interactable = false;
    }
}
