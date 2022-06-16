using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class LoseScreenController : MonoBehaviour
{
   public void ReturnToSplash() {
        SceneManager.LoadScene(Constants.SPLASH_SCENE);
    }
}
