using UnityEngine;

public class LoadingMainScene : MonoBehaviour
{


    public void onLoadingShowed()
    {
        SceneTransition.SwitchToScene("Game");
    }
    
}
