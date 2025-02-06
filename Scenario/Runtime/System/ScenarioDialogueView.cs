using Cysharp.Threading.Tasks;
using DG.Tweening;
using PCS.Common;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioDialogueView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private RectTransform _speakerView;
    [SerializeField] private TextMeshProUGUI _speakerName;
    [SerializeField] private TextMeshProUGUI _speakerTitle;
    [SerializeField] private Image _dialogueEnd;
    [SerializeField] private float _endFadeDuration;
    [SerializeField] private int _maxVisibleLine = 4;
    [Tooltip("Second")][SerializeField] private float _typingSpeed;

    private UniTaskCompletionSource<bool> _completionSource;
    private string _desc;
    private CancellationTokenSource _cts;

    public void Initialize()
    {
        _dialogueText.maxVisibleLines = _maxVisibleLine;
    }

    public UniTask<bool> Show(ScenarioScript script)
    {
        HideDialogueEnd();
        _completionSource = new UniTaskCompletionSource<bool>();
        _cts = new CancellationTokenSource();

        _dialogueText.SetText(string.Empty);
        _speakerName.SetText(LanguageManager.GetLanguage(script.SpeakerNameKey));
        _speakerTitle.SetText(LanguageManager.GetLanguage(script.SpeakerTitleKey));
        _desc = LanguageManager.GetLanguage(script.DialogKey);

        gameObject.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_speakerView);
        TypeAnimation().Forget();

        return _completionSource.Task.AttachExternalCancellation(this.GetCancellationTokenOnDestroy());
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public async UniTask TypeAnimation()
    {
        float duration = _desc.Length * _typingSpeed;
        await _dialogueText.DOText(_desc, duration).SetEase(Ease.Linear).WithCancellation(_cts.Token);
        ShowDialogueEnd();
        _completionSource.TrySetResult(true);
    }


    public void SkipTypingAnimation()
    {
        CancelCts();
        _dialogueText.SetText(_desc);
        ShowDialogueEnd();
        _completionSource.TrySetResult(true);
    }

    private void CancelCts()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
    public void ShowDialogueEnd()
    {
        _dialogueEnd.DOKill();
        _dialogueEnd.DOFade(1, _endFadeDuration);
    }
    public void HideDialogueEnd()
    {
        _dialogueEnd.DOKill();
        _dialogueEnd.color = new Color(_dialogueEnd.color.r, _dialogueEnd.color.g, _dialogueEnd.color.b, 0f);
    }
}
