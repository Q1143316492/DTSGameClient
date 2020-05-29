using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWLEngine.GameImpl.Base
{
    public enum EActionEvent
    {
        EMPTY,
        DOWN,
        UP,
    };

    public class ActionEvent
    {
        public EActionEvent state = EActionEvent.EMPTY;
        public float lastTime = 0;
    }
}
