using UnityEngine;
using System.Collections;
using LarkFramework.GameFollow;
using LarkFramework;

namespace EasySpace
{

    public class My_GameMode : GameModeBase<My_GameMode,My_GameInstance>
    {

        [HideInInspector]
        public My_GameInstance gameInstance { get; private set; }
        [HideInInspector]
        public GameObject gameInstanceObj { get; private set; }

        public override void Init(My_GameInstance gameInstance, GameObject obj)
        {
            base.Init(gameInstance, obj);

            this.gameInstance.onUpdate += OnUpdate;

            //Init各类管理器
            //My_ScenesMgr.Create().Init();

            LarkLog.Log(this.name + " Init Finished");
        }
    }
}
