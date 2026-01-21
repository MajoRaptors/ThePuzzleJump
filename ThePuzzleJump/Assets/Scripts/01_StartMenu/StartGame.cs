using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class StartGame : MonoBehaviour
{
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            LaunchGame();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            LaunchGame();
        }
    }

    void LaunchGame()
    {
        SceneManager.LoadScene("02_EnigmaScene");
    }
}