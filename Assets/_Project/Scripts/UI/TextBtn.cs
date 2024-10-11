using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    // Named like this because unity won't accept this as a facade on a object with a Button component. Go figure
    public class TextBtn : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonTMP;

        public TextMeshProUGUI TMP => _buttonTMP == null ? _buttonTMP = GetComponent<TextMeshProUGUI>() : _buttonTMP;
        public Button Button => _button == null ? _button = GetComponent<Button>() : _button;

        public string Text
        {
            get => _buttonTMP.text;
            set => _buttonTMP.text = value;
        }

        public Color TextColor
        {
            get => _buttonTMP.color;
            set => _buttonTMP.color = value;
        }
    }
}
