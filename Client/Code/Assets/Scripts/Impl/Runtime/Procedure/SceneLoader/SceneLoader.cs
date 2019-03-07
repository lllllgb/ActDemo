using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AosHotfixFramework;

namespace AosHotfixRunTime
{
    public class SceneLoader : Singleton<SceneLoader>
    {
        System.Action mFinishHandle;

        public void LoadScene(string name, System.Action finishHandle)
        {
            mFinishHandle = finishHandle;
            Game.ResourcesMgr.LoadBundleByType(EABType.Scene, name);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(name);
        }

        public async void LoadSceneAsync(string name, System.Action finishHandle)
        {
            mFinishHandle = finishHandle;
            await Game.ResourcesMgr.LoadBundleByTypeAsync(EABType.Scene, name);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadSceneAsync(name);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            mFinishHandle?.Invoke();
            mFinishHandle = null;
        }
    }
}
