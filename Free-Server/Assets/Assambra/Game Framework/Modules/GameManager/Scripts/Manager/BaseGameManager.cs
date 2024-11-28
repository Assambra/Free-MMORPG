using System;
using System.Collections;
using UnityEngine;

namespace Assambra.GameFramework.GameManager
{
    public abstract class BaseGameManager : MonoBehaviour
    {
        public static event Action<GameState> OnStateChanged;
        [field: SerializeField] public SceneHandler sceneHandler { get; private set; }

        [SerializeField] public GameState _currentState = GameState.None;
        [Tooltip("We need a small time delay so that we can use the OnStateChanged event in UI elements, as they are only being instantiated to use the event.")]
        [SerializeField] private float _timeDelayChangeGameState = 0.1f;

        private void OnEnable()
        {
            SceneHandler.OnSceneChanged += OnSceneChanged;
        }

        private void OnDisable()
        {
            SceneHandler.OnSceneChanged -= OnSceneChanged;
        }

        public void ChangeState(GameState newState)
        {
            StartCoroutine(ChangeStateRoutine(newState));
        }

        IEnumerator ChangeStateRoutine(GameState newState)
        {
            yield return new WaitForSeconds(_timeDelayChangeGameState);

            if (_currentState != newState)
            {
                _currentState = newState;
                OnStateChanged?.Invoke(newState);
            }
        }

        public void ChangeScene(Scenes scene)
        {
            sceneHandler.ChangeScene(sceneHandler.Scenes[(int)scene]);
        }

        public Scenes getScenesByName(string scenename)
        {
            return sceneHandler.GetScenesByName(scenename);
        }

        protected abstract void OnSceneChanged(Scene lastScene, Scene newScene);
    }
}
