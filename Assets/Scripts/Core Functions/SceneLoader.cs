using UnityEngine;
using UnityEngine.SceneManagement;
using QFSW.QC;
using QFSW.QC.Suggestors.Tags;
using Mono.CSharp;
using Unity.Netcode;


public class SceneLoader : NetworkBehaviour
{
    [Command]
    public void ChangeScene([SceneName] string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    [Command] [ClientRpc]
    public void ChangeSceneClientRPC([SceneName] string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}