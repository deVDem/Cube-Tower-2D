using Game_Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    private static SceneTransition instance;

    public Animator componentAnimator;
    private AsyncOperation loadingSceneOperation;
    public Image progressImage;
    private bool _isprogressImageNotNull;


    public static void sendSceneClosing()
    {
        instance.componentAnimator.SetTrigger("sceneClosing");
    }

    public static void SwitchToScene(string sceneName)
    {
        sendSceneClosing();
        instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
        instance.loadingSceneOperation.allowSceneActivation = false;
    }

    void Start()
    {
        _isprogressImageNotNull = progressImage != null;
        instance = this;
        if (componentAnimator == null) componentAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (loadingSceneOperation != null)
        {
            if (_isprogressImageNotNull)
                progressImage.fillAmount = progressImage.fillAmount +
                                           (loadingSceneOperation.progress / 0.9f -
                                           progressImage.fillAmount) * Time.deltaTime;
            if (loadingSceneOperation.progress >= 0.9f) componentAnimator.SetTrigger("HideLoading");
        }
    }


    public void onAnimationOver()
    {
        if (loadingSceneOperation != null) loadingSceneOperation.allowSceneActivation = true;
    }

    public void quit()
    {
        GameController.quit();
    }
}