using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class TextBtnImage : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _buttonTMP;
        [SerializeField] private Image _image;
        [SerializeField] private Button _button;

        public TextMeshProUGUI TMP => _buttonTMP == null ? _buttonTMP = GetComponent<TextMeshProUGUI>() : _buttonTMP;
        public Button Button => _button == null ? _button = GetComponent<Button>() : _button;
        public Image Image => _image == null ? _image = GetComponent<Image>() : _image;

        public string Text
        {
            get => _buttonTMP.text;
            set => _buttonTMP.text = value;
        }

        public Sprite Sprite
        {
            get => _image.sprite;
            set => _image.sprite = value;
        }

        public Color TextColor
        {
            get => _buttonTMP.color;
            set => _buttonTMP.color = value;
        }

        public Color ImageColor
        {
            get => _image.color;
            set => _image.color = value;
        }
    }
}