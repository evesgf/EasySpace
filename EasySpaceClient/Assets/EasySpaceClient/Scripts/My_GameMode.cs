using UnityEngine;
using System.Collections;
using LarkFramework.GameFollow;
using LarkFramework;

namespace EasySpace
{

    public class My_GameMode : GameModeBase<My_GameMode>
    {

        [HideInInspector]
        public My_GameInstance gameInstance { get; private set; }
        [HideInInspector]
        public GameObject gameInstanceObj { get; private set; }

        public virtual void Init(My_GameInstance gameInstance)
        {
            this.gameInstance = gameInstance;
            this.gameInstanceObj = gameInstance.gameObject;

            this.gameInstance.onUpdate += OnUpdate;

            //Init各类管理器
            //My_ScenesMgr.Create().Init();

            LarkLog.Log(this.name + " Init Finished");
        }

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return this.gameInstance.StartCoroutine(routine);
        }

        public void Coroutine(IEnumerator routine)
        {
            this.gameInstance.StopCoroutine(routine);
        }
    }
}
