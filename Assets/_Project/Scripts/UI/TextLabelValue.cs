using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextLabelValue : MonoBehaviour
    {
        [SerializeField] private bool _log = false;

        private TextMeshProUGUI _tmp;

        private string _originalText;
        private bool _initialized = false;

        public string Text
        {
            get => _tmp.text;
            set => _tmp.text = value;
        }

        /// <summary>
        /// Replace strings that starts with $ with args. Has to be passed in order.
        /// </summary>g
        /// <param name="args"></param>
        public void Setup(params string[] args)
        {
            if (!_initialized)
            {
                Initialize();
            }

            string text = _originalText;
            MatchCollection matches = Regex.Matches(text, @"(\$[^ \n<>]*)", RegexOptions.Multiline);

            if (_log)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    Debug.Log($"Args: {args[i]}");
                }

                for (int i = 0; i < matches.Count; i++)
                {
                    Debug.Log($"Match: {matches[i].Value}");
                }
            }

            for (int i = 0; i < args.Length && i < matches.Count; i++)
            {
                text = text.Replace(matches[i].Value, args[i]);
            }


            _tmp.text = text;
        }

        private void Initialize()
        {
            _tmp = GetComponent<TextMeshProUGUI>();
            _originalText = _tmp.text;
            _initialized = true;
        }
    }
}