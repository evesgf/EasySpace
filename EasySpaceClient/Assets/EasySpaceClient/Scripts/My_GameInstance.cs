using UnityEngine;
using System.Collections;
using LarkFramework.GameFollow;
using LarkFramework;

namespace EasySpace
{
    public class My_GameInstance : GameInstanceBase<My_GameInstance>
    {

        void Awake()
        {

        }

        public My_GameInstance Init()
        {
            //初始化GameMode
            My_GameMode.Create().Init(this,gameObject);

            LarkLog.Log(this.name + " Init Finished");
            return this;
        }

        void OnApplicationQuit()
        {

        }

        void OnApplicationPause(bool pause)
        {

        }
    }
}
