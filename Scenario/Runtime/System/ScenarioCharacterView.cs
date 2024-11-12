using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioCharacterView : MonoBehaviour
{
    [SerializeField] private Image _characterImage;
    //[SerializeField] private int _maxCharacterCount = 4;
    [SerializeField] private Color _dimColor = new Color(0.5f,0.5f,0.5f,1.0f);
    private List<Image> _characterImages = new List<Image>();

    public void Show(List<ImageListInfo> infos)
    {
        foreach(var v in _characterImages)
            Destroy(v.gameObject);
        _characterImages.Clear();
        Image character;
        EEmotionType emotionType;
        //프토로 타입이므로 생성 파괴로 구현. 필요시 Pooling으로 변경.
        for(int i=0; i< infos.Count; i++)
        {
            character = Instantiate(_characterImage,_characterImage.transform.parent);

            character.sprite = Resources.Load<Sprite>(infos[i].ImageName);
            emotionType = infos[i].EmotionType;

            character.SetNativeSize();
            character.rectTransform.localScale = infos[i].Scale;
            character.rectTransform.localPosition = infos[i].Position;
            if (infos[i].IsDimming)
                SetDim(character);
            character.gameObject.SetActive(true);
            _characterImages.Add(character);
        }
    }

    private void SetDim(Image image)
    {
        Color currentColor = image.color;
        image.color = currentColor * _dimColor;
    }
}
