using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {

    public string sceneToLoad;

    void Start() {
        SceneManager.LoadScene(sceneToLoad);
    }
}
