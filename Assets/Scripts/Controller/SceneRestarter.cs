using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SceneRestarter : MonoBehaviour
{
    [SerializeField] private Slider restartProgressBar;
    [SerializeField] private float holdTime = 1.5f;
    
    private float _currentHoldTime;
    private bool _isRestarting;

    private void Update()
    {
        if (Keyboard.current.tabKey.isPressed)
        {
            _isRestarting = true;
            _currentHoldTime += Time.deltaTime;

            if (restartProgressBar != null)
            {
                restartProgressBar.gameObject.SetActive(true);
                restartProgressBar.value = _currentHoldTime / holdTime;
            }

            if (_currentHoldTime >= holdTime)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else
        {
            if (_isRestarting)
            {
                _isRestarting = false;
                _currentHoldTime = 0;
                if (restartProgressBar != null)
                {
                    restartProgressBar.value = 0;
                    restartProgressBar.gameObject.SetActive(false);
                }
            }
        }
    }
}