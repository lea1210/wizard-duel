using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{

    [SerializeField]
    Player player;
    [SerializeField]
    ArenaScenes.ArenaScene playerDeathScene;

    [SerializeField]
    Enemy enemy;
    [SerializeField]
    ArenaScenes.ArenaScene enemyDeathScene;


    [SerializeField]
    Animator playerFadeAnimator;
    ArenaScenes.ArenaScene sceneToLoad;



    void Update()
    {
        if (enemy != null)
        {
            if (enemy.GetCurrentHealth() == 0)
            {
                sceneToLoad = enemyDeathScene;
                player.playFadeOut();
            }
            else if (player.GetCurrentHealth() == 0)
            {

                player.playFadeOut();
                sceneToLoad = playerDeathScene;
            }
        }
        
        if (playerFadeAnimator.GetCurrentAnimatorStateInfo(0).IsName("BlackScreen")){
            SceneManager.LoadSceneAsync(sceneToLoad.ToString());
        }
    }

    public void invokePoratelCollision(ArenaScenes.ArenaScene scene)
    {

        player.playFadeOut();
        sceneToLoad = scene;

    }
}
