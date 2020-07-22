using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Screeps_API;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Screeps3D
{
    public class GameManager : BaseSingleton<GameManager>
    {
        public static GameMode CurrentMode { get; private set; }
        public static event Action<GameMode> OnModeChange;

        [SerializeField] private GameMode _defaultMode = default;
        [SerializeField] private FadePanel _exitCue = default;

        public Dictionary<string, Color> PlayerColors { get; private set; }

        public GameManager()
        {
            this.PlayerColors = new Dictionary<string, Color>();
        }

        public override void Awake()
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);

            if (_defaultMode != GameMode.Login && !ScreepsAPI.IsConnected)
                SceneManager.LoadScene(0);
            else
                OnModeChange = null;
            
            base.Awake();
        }

        private void Start()
        {
            if (ScreepsAPI.Console == null)
            {
                SceneManager.LoadScene(0);
            }

            ScreepsAPI.OnConnectionStatusChange += OnConnectionStatusChange;
            _exitCue.OnFinishedAnimation += OnExitCue;

            Init();
        }

        private void Init()
        {
            PoolLoader.Init();
            PrefabLoader.Init();

            StartCoroutine(LoadOptions());
        }

        private IEnumerator LoadOptions()
        {
            yield return new WaitForSeconds(1);

            SceneManager.LoadSceneAsync("Scenes/Options", LoadSceneMode.Additive);
        }

        private void OnDestroy()
        {
            ScreepsAPI.OnConnectionStatusChange -= OnConnectionStatusChange;
        }

        private void OnExitCue(bool isVisible)
        {
            if (isVisible || _defaultMode == CurrentMode)
                return;
            
            if (CurrentMode == GameMode.Login)
            {
                SceneManager.LoadScene(0);
            }

            if (CurrentMode == GameMode.Room)
            {
                SceneManager.LoadScene(1);
            }
        }

        private void Update()
        {
            ChangeMode(_defaultMode);
            enabled = false;
        }

        private void OnConnectionStatusChange(bool isConnected)
        {
            if (isConnected)
            {
                ChangeMode(GameMode.Room);
            }
            else
            {
                ChangeMode(GameMode.Login);
            }
        }

        public static void ChangeMode(GameMode mode)
        {
            CurrentMode = mode;
            if (OnModeChange != null)
                OnModeChange(mode);
        }
    }
    
    public enum GameMode
    {
        Login,
        Options,
        Room,
        Map,
    }
}