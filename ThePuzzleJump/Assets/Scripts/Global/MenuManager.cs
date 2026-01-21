using UnityEngine;
using UnityEngine.EventSystems;
public class MenuManager : MonoBehaviour
{
    public GameObject globalMenuPanel; // assigne le panel ici
    private bool isMenuOpen = false;

    void Awake()
    {
        
        DontDestroyOnLoad(this.gameObject); // persistant entre toutes les scènes
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        globalMenuPanel.SetActive(isMenuOpen);
        // Time.timeScale = isMenuOpen ? 0f : 1f; // si on veut figer le jeu
    }

    public void CloseMenu()
    {
        isMenuOpen = false;
        globalMenuPanel.SetActive(false);
        // Time.timeScale = 1f;
    }
}