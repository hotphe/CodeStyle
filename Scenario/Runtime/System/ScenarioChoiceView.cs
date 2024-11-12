using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioChoiceView : MonoBehaviour
{
    [SerializeField] private GameObject _dim;
    [SerializeField] private ChoiceHolder _choiceHolder;
    
    private List<ChoiceHolder> _choiceHolders = new List<ChoiceHolder>();
    private List<UniTask> _buttonTaskList = new List<UniTask>();

    public async UniTask<int> ShowAsync(List<ScenarioChoice> choices )
    {
        ChoiceHolder choiceHolder;
        for(int i =0; i<choices.Count; i++)
        {
            choiceHolder = Instantiate(_choiceHolder,_choiceHolder.transform.parent);
            choiceHolder.OnButtonClick += OnButtonClick;
            _choiceHolders.Add(choiceHolder);
            _buttonTaskList.Add(choiceHolder.Initialize(choices[i]));
            choiceHolder.gameObject.SetActive(true);
        }
        
        gameObject.SetActive(true);
        int index = await UniTask.WhenAny(_buttonTaskList).AttachExternalCancellation(this.GetCancellationTokenOnDestroy());

        //프로토타입이므로 생성 파괴하는 형식으로 구현. 추후 pooling 필요시 변경.
        foreach (var v in _choiceHolders)
            Destroy(v.gameObject);

        _choiceHolders.Clear();
        _buttonTaskList.Clear();

        return choices[index].ScriptID;
    }

    private void OnButtonClick()
    {
        foreach (var choiceHolder in _choiceHolders)
            choiceHolder.IsSelected = false;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
