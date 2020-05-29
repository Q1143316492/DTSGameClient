using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Entity
{
    public enum CharacterType
    {
        UNDEFINE,
        PLAYER,
        OTHER_PLAYER,
        COMPUTER,
    }

    public class CharacterBase
    {
        protected CharacterType charactType = CharacterType.UNDEFINE;

        protected int userID = -1;
        protected int roomID = -1;

        public int GetRoomID()
        {
            return roomID;
        }

        public void SetRoomID(int roomID)
        {
            this.roomID = roomID;
        }

        public int GetUserID()
        {
            return userID;
        }

        public CharacterType GetCharaterType()
        {
            return charactType;
        }

        public virtual Transform GetTransform() { return null; }
        public virtual void SetTransform(Vector3 position, Vector3 rotation) { }
        public virtual void SetPosition(Vector3 position) { }
        public virtual void SetRotation(Vector3 rotation) { }

        public virtual void UpdateOperation(PlayerAction playerAction) { }
        public virtual void Hit(int hp) { }
        public virtual void SetHp(int hp) { }
        public virtual float GetHP() { return 1; }
        public virtual bool Death() { return false; }
        public virtual void GoToDeath() { }

        public virtual void Destory() { }
    }
}
