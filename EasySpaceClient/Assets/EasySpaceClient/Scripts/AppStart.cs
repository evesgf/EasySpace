using UnityEngine;
using System.Collections;

namespace EasySpace
{
    public class AppStart : MonoBehaviour
    {
        public AppEnums.AppMode mode;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(AppLaunching());
        }

        IEnumerator AppLaunching()
        {
            //配置加载
            //FPS
            Application.targetFrameRate = 60;

            //日志输出


            if (My_GameInstance.Instance == null)
            {
                yield return My_GameInstance.Create().Init();
            }

            //进入测试逻辑
            switch (mode)
            {
                case AppEnums.AppMode.Developing:
                    {
                        yield return null;
                    }
                    break;
                case AppEnums.AppMode.QA:
                    {
                        yield return null;
                    }
                    break;
                case AppEnums.AppMode.Release:
                    {
                        yield return null;
                    }
                    break;
            }
        }
    }

}