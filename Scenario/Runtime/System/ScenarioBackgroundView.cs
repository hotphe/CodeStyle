using UnityEngine;
using UnityEngine.UI;

public class ScenarioBackgroundView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
    public void ChangeSprite(string path)
    {
        if (string.IsNullOrEmpty(path))
            _backgroundImage.sprite = null;
        else
        {
            Sprite sprite = Resources.Load<Sprite>(path);
            _backgroundImage.sprite = sprite;
        }
    }
}
