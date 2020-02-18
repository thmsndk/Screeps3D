using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class PlayerInput : BaseSingleton<PlayerInput>
    {
        [SerializeField] private FadePanel _panel;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Button _submit;
        [SerializeField] private Button _cancel;
        private Action<string> _onInput;
        private Action<bool> _onAnswerQuestion;
        private TextMeshProUGUI _submitText;

        public static void Get(string label, Action<string> onInput)
        {
            Instance.GetInput(label, onInput);
        }

        private void Start()
        {
            _submitText = _submit.GetComponentInChildren<TextMeshProUGUI>();
            _submit.onClick.AddListener(OnSubmit);
            _input.onSubmit.AddListener(OnSubmit);
            _cancel.onClick.AddListener(OnCancel);
        }

        private void OnCancel()
        {
            if (_onInput != null)
            {
                _onInput(null);

                _onInput = null;
            }

            if (_onAnswerQuestion != null)
            {
                _onAnswerQuestion(false);
                _onAnswerQuestion = null;
            }

            _panel.Hide();
        }

        private void OnSubmit()
        {
            if (_onInput != null)
            {
                OnSubmit(_input.text);

                _onInput = null;
            }

            if (_onAnswerQuestion != null)
            {
                _onAnswerQuestion(true);
                _onAnswerQuestion = null;
                _panel.Hide();
            }
        }

        private void OnSubmit(string text)
        {
            _onInput(text);
            _panel.Hide();
        }

        private void GetInput(string label, Action<string> onInput)
        {
            if (_submitText != null)
            {
                _submitText.text = "Submit";
            }

            _onInput = onInput;
            _input.gameObject.SetActive(true);
            _label.text = label;
            _panel.Show();
        }

        private void Question(string label, Action<bool> onAnswerQuestion)
        {
            if (_submitText != null)
            {
                _submitText.text = "Yes";
            }

            _onAnswerQuestion = onAnswerQuestion;
            _input.gameObject.SetActive(false);
            _label.text = label;
            _panel.Show();
        }

        public static void AskQuestion(string label, Action<bool> onAnswerQuestion)
        {
            Instance.Question(label, onAnswerQuestion);
        }
    }
}