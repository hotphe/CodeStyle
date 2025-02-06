using Cysharp.Threading.Tasks;
using PCS.Common;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceHolder : MonoBehaviour
{
    public event Action OnButtonClick;

    [SerializeField] private Sprite _unselectedSprite;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;
    private UniTaskCompletionSource _completionSource;
    private bool _isSelected = false;
    public bool IsSelected
    {
        get 
        { 
            return _isSelected;
        }
        set
        {
            if(value)
                _button.image.sprite = _selectedSprite;
            else
                _button.image.sprite = _unselectedSprite;

            _isSelected = value;
        }
    }

    public UniTask Initialize(ScenarioChoice choice)
    {
        _text.SetText(LanguageManager.GetLanguage(choice.DescKey));
        _completionSource = new UniTaskCompletionSource();
        _button.onClick.AddListener(OnClickButton);
        return _completionSource.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
    }
    // 한번 클릭하면 이미지 변환, 두번 클릭하면 선택확정
    private void OnClickButton()
    {
        if(!_isSelected)
        {
            OnButtonClick?.Invoke();
            IsSelected = true;
        }else
        {
            _completionSource.TrySetResult();
        }
    }

    

}
