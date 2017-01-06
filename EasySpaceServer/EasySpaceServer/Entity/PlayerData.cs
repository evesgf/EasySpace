using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySpaceServer.Entity
{
    [Serializable]
    public class PlayerData
    {
        public int score = 0;

        //临时数据
        public PlayerTempData tempData;

        public PlayerData()
        {
            score = 100;
        }
    }
}
