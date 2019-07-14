using System;
using System.Collections.Generic;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Unity_Console
{
    public class UnityConsole : MonoBehaviour
    {
        struct Message
        {
            public Color color;
            public string text;
        }

        public Action<string> OnInput;
    
        [SerializeField] private ConsoleLine _prototype;
        [SerializeField] private TMP_InputField _input;
        [SerializeField] public FadePanel _panel;
        [SerializeField] public RectTransform _content;
        private List<ConsoleLine> _lines = new List<ConsoleLine>();
        private Queue<Message> _messages = new Queue<Message>();
        private float _nextMessage;
        private List<string> _inputLog = new List<string>();
        private int _cycleIndex;
        private float _cycleDelay;
        private float _nextCycle;
        private string _currentMsg;

        private void Start()
        {
            _prototype.Hide();
            _input.onSubmit.AddListener(OnSubmit);
        }

        private void OnSubmit(string msg)
        {
            if (msg.Length == 0 || _input.wasCanceled)
                return;
        
            if (OnInput != null) OnInput.Invoke(msg);
            AddMessage(string.Format("> {0}", msg), Color.cyan);
            _input.text = "";
            _inputLog.Add(msg);
            _cycleIndex = _inputLog.Count;
            _input.ActivateInputField();
        }

        public void AddMessage(string msg, Color color)
        {
            _messages.Enqueue(new Message() {text = msg, color = color});
        }

        private void Update()
        {
            DisplayMessages();
            CycleMessages();

            if (!EventSystem.current.currentSelectedGameObject && Input.GetKey(KeyCode.Return))
            {
                _input.ActivateInputField();
            }
        }

        private void CycleMessages()
        {
            if (!_input.isFocused)
                return;

            var delta = 0;
            if (Input.GetKey(KeyCode.UpArrow))
            {
                delta = -1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                delta = 1;
            }
            else
            {
                _nextCycle = 0;
            }

            if (delta == 0)
            {
                _cycleDelay = .5f;
                return;
            }

            if (_nextCycle > Time.time)
                return;
            _nextCycle = Time.time + _cycleDelay;
            _cycleDelay -= .1f;
            _cycleDelay = Mathf.Max(_cycleDelay, .05f);

            if (_cycleIndex == _inputLog.Count)
            {
                _currentMsg = _input.text;
            }

            _cycleIndex += delta;
            if (_cycleIndex < 0)
            {
                _cycleIndex = 0;
                return;
            }

            if (_cycleIndex >= _inputLog.Count)
            {
                _cycleIndex = _inputLog.Count;
                _input.text = _currentMsg;
                return;
            }

            _input.text = _inputLog[_cycleIndex];
            _input.selectionStringAnchorPosition = 0;
            _input.selectionStringFocusPosition = _input.text.Length;
        }

        private void DisplayMessages()
        {
            if (_nextMessage > Time.time)
            {
                return;
            }

            if (_messages.Count == 0)
            {
                return;
            }

            _nextMessage = Time.time + .1f / _messages.Count;
            var displayCount = Mathf.Max(_messages.Count / 10, 1);
            for (var i = 0; i < displayCount; i++)
            {
                var message = _messages.Dequeue();
                var line = GetLine();
                line.Text = message.text;
                line.Color = message.color;
                line.Show();
            }

            SetGeometry();
        }

        private void SetGeometry()
        {
            float height = 5;
            for (var i = 0; i < _lines.Count; i++)
            {
                var line = _lines[_lines.Count - 1 - i];
                var rect = line.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(0, height);
                height += line.Height;
            }
            _content.sizeDelta = new Vector2(0, height);
        }

        private ConsoleLine GetLine()
        {
            if (_lines.Count < 100)
            {
                var line = Instantiate(_prototype.gameObject).GetComponent<ConsoleLine>();
                line.transform.SetParent(_prototype.transform.parent);
                _lines.Add(line);
                return line;
            } else
            {
                var line = _lines[0];
                _lines.RemoveAt(0);
                _lines.Add(line);
                return line;
            }
        }
    }
}