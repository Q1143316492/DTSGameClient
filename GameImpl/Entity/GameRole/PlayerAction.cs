using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Entity
{
    public enum EPlayerAction
    {
        SHOOT,
        SHOOT_BURST,

        RELOAD,

        BEGIN_RUN,
        END_RUN,
        BEGIN_CROUCH,
        END_CROUCH,

        DUMP,

        SHOOT_LINE,

        CHANGE_WEAPON
    }

    public class PlayerAction
    {
        public int actionSign = 0;

        private float mouseX = 0;
        private float mouseY = 0;
        private float MoveH = 0;
        private float MoveV = 0;

        public WeaponType ChangedWeaponType;

        public PlayerAction()
        {
            Init();
        }

        public void Init()
        {
            actionSign = 0;
            mouseX = mouseY = MoveH = MoveV = 0;
        }

        public KeyValuePair<float, float> GetMouseXY()
        {
            return new KeyValuePair<float, float>(mouseX, mouseY);
        }

        public KeyValuePair<float, float> GetMoveHV()
        {
            return new KeyValuePair<float, float>(MoveH, MoveV);
        }

        public float GetMouseX() { return mouseX; }
        public float GetMouseY() { return mouseY; }
        public float GetMoveH() { return MoveH; }
        public float GetMoveV() { return MoveV; }

        public void Add(EPlayerAction action)
        {
            actionSign |= (1 << (int)action);
        }

        public void Del(EPlayerAction action)
        {
            actionSign &= (~(1 << (int)action));
        }

        public int Get(EPlayerAction action)
        {
            return Get((int)action);
        }

        public int Get(int bit)
        {
            return (actionSign >> bit) & 1;
        }

        public void SetMouse(float x, float y)
        {
            mouseX = x;
            mouseY = y;
        }

        public void SetMove(float h, float v)
        {
            MoveH = h;
            MoveV = v;
        }

        public void SetMoveH(float h) { MoveH = h; }
        public void SetMoveV(float v) { MoveV = v; }

        public void SetMouseX(float x) { mouseX = x; }
        public void SetMouseY(float y) { mouseY = y; }

        public string ToNetworkString()
        {
            return string.Format("{0}|{1:F}|{2:F}|{3:F}|{4:F}", actionSign.ToString(), MoveH, MoveV, mouseX, mouseY);
        }
    }
}
