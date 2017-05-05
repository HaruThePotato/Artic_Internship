using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSkipper : MonoBehaviour {

    public void gotoTrainerMode() {
        SceneManager.LoadScene(1);
    }

    public void gotoTraineeMode()
    {
        SceneManager.LoadScene(2);
    }
}
