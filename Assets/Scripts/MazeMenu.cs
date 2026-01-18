using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MazeMenu : MonoBehaviour
{
    public InputField roomsInput;
    public InputField entranceIndexInput;
    public Button generateButton;

    void Start()
    {
        generateButton.onClick.AddListener(OnGenerateClicked);
    }

    void OnGenerateClicked()
    {
        // Guardar parámetros en MazeSettings
        if (int.TryParse(roomsInput.text, out int maxRooms))
            MazeSettings.maxRooms = Mathf.Max(1, maxRooms);

        if (int.TryParse(entranceIndexInput.text, out int entranceIndex))
            MazeSettings.entranceIndex = Mathf.Max(0, entranceIndex);

        // Cargar escena del laberinto
        SceneManager.LoadScene("GameScene");
    }
}
