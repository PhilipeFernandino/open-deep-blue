using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class ImageText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _tmp;
        [SerializeField] private Image _image;

        public TextMeshProUGUI TMP => _tmp == null ? _tmp = GetComponent<TextMeshProUGUI>() : _tmp;
        public Image Image => _image == null ? _image = GetComponent<Image>() : _image;

        public string Text
        {
            get => _tmp.text;
            set => _tmp.text = value;
        }

        public Sprite Sprite
        {
            get => _image.sprite;
            set => _image.sprite = value;
        }

        public Color TextColor
        {
            get => _tmp.color;
            set => _tmp.color = value;
        }

        public Color ImageColor
        {
            get => _image.color;
            set => _image.color = value;
        }
    }
}