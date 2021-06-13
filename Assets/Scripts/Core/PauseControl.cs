using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Game.Input;
using Game.Utils;
using Game.Player;


namespace Game.Core {
    [RequireComponent(typeof(Overlay))]
    public class PauseControl : MonoBehaviour
    {
        // Constants
        private enum PauseType {
            Title,
            GameOver,
            Pause,
            None
        }
        private float ON_SCREEN_UI = 9;
        private float OFF_SCREEN_UI = 12;
        private float UI_MOVE_TIME = .4f;
        
        // Variables
        private PauseType type = PauseType.Title;
        private bool paused = true;
        
        // Components & References
        public GameObject ui_container;
        public PlayerController player;
        public InputController inputController;
        public GameObject[] canvases = new GameObject[4];
        
        private Overlay overlay; // 0 title, 1 pause, 2 gameOver


        private void Awake() {
            overlay = GetComponent<Overlay>();
        }

        private void Start() {
            overlay.SetState(paused);

            foreach (GameObject obj in canvases) {
                obj.SetActive(false);
            }
            AppearCanvas(canvases[0]);
        }
        
        private void Update() {
            switch (type) {
                case (PauseType.Title):
                    if (inputController.GetKeyDown(KeyCode.Space) || (inputController.GetMovementInput() != Vector2.zero)) {
                        StartCoroutine(StartGame());
                        type = PauseType.None;
                    }
                    break;
                case (PauseType.GameOver):
                    break;
                case (PauseType.None):
                case (PauseType.Pause):
                    if (inputController.GetKeyDown(KeyCode.Escape) || inputController.GetKeyDown(KeyCode.P)) {
                        if (paused) StartCoroutine(RegularUnpause());
                        else StartCoroutine(RegularPause());
                    }
                    if (!player.isAlive()) BeginGameOver();
                    break;
            }   
        }

        private void AppearCanvas(GameObject obj) {
            obj.SetActive(true);
            // TODO: Effects for appearance !!!
        }

        private void DisappearCanvas(GameObject obj) {
            obj.SetActive(false);
            // TODO: Effects for disappearance !!!
        }

        #region Pause
            private IEnumerator RegularPause() {
                yield return StartCoroutine(Pause());
                type = PauseType.Pause;
                AppearCanvas(canvases[1]);
            }

            private IEnumerator Pause() {
                paused = true;
                inputController.MouseEnable(false);
                LerpUtils.LerpDelegate SlideUI = (lerpTime) => {
                    Vector3 pos = ui_container.transform.position;
                    pos.x = Mathf.Lerp(ON_SCREEN_UI, OFF_SCREEN_UI, lerpTime);
                    pos.y = 0;
                    pos.z = 10;
                    ui_container.transform.localPosition = pos;
                };
                StartCoroutine(LerpUtils.LerpCoroutine(SlideUI, 0, 1, UI_MOVE_TIME));
                yield return StartCoroutine(overlay.RaiseOverlay());
            }

            public void UnpauseFunction() {StartCoroutine(RegularUnpause()); }

            private IEnumerator RegularUnpause() {
                yield return StartCoroutine(Unpause());
                type = PauseType.None;
                DisappearCanvas(canvases[1]);
            }

            private IEnumerator Unpause() {
                paused = false;
                inputController.MouseEnable(true);
                LerpUtils.LerpDelegate SlideUI = (lerpTime) => {
                    Vector3 pos = ui_container.transform.position;
                    pos.x = Mathf.Lerp(OFF_SCREEN_UI, ON_SCREEN_UI, lerpTime);
                    pos.y = 0;
                    pos.z = 10;
                    ui_container.transform.localPosition = pos;
                };
                StartCoroutine(LerpUtils.LerpCoroutine(SlideUI, 0, 1, UI_MOVE_TIME));
                yield return StartCoroutine(overlay.LowerOverlay());
            }
        #endregion

        #region Title
            private IEnumerator StartGame() {
                yield return StartCoroutine(Unpause());
                DisappearCanvas(canvases[0]);
            }
        #endregion

        #region GameOver
            public void BeginGameOver() {
                Pause();
                type = PauseType.GameOver;
                AppearCanvas(canvases[2]);
                overlay.SetState(true);
            }

            public void RestartGame() {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            public void QuitGame() {
                Application.Quit();
            }
        #endregion

        #region Win Screen
            public void WinGame() {
                StartCoroutine(WinScreen());
            }

            private IEnumerator WinScreen() {
                yield return StartCoroutine(Pause());
                AppearCanvas(canvases[3]);
            }
        #endregion
    }
}