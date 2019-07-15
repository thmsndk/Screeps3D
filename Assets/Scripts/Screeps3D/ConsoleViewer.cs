using System;
using System.IO;
using UnityEngine;
using Screeps3D;
using Screeps_API;
using Unity_Console;
using System.Text.RegularExpressions;
using Assets.Scripts.Screeps3D;

namespace Screeps3D
{
    public class ConsoleViewer : MonoBehaviour
    {
        [SerializeField] private UnityConsole _console;

        private void Start()
        {
            _console.OnInput += OnInput;
            ScreepsAPI.Console.OnConsoleMessage += OnMessage;
            ScreepsAPI.Console.OnConsoleError += OnError;
            ScreepsAPI.Console.OnConsoleResult += OnResult;
            
            GameManager.OnModeChange += OnModeChange;
        }

        private void OnModeChange(GameMode mode)
        {
            _console._panel.SetVisibility(mode != GameMode.Login);
        }

        private void OnInput(string obj)
        {
            ScreepsAPI.Console.Input(obj);
        }

        private void OnMessage(ScreepsConsole.ConsoleMessage message)
        {
            PrintMessage(message, Color.white);
        }

        private void OnError(string obj)
        {
            PrintMessage(obj, Color.red);
        }

        private void OnResult(string obj)
        {
            PrintMessage(obj, Color.green);
        }

        private void PrintMessage(string message, Color color)
        {
            var reader = new StringReader(message);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _console.AddMessage(line, color);
            }
        }
    }
}