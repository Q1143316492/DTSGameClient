using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Controller.Weapon;
using CWLEngine.GameImpl.Controller.SoldierState;
using CWLEngine.GameImpl.Network;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Util;
using System.Reflection;

namespace CWLEngine.GameImpl.Controller
{
    public class PlayersController
    {
        readonly int[,] Next = new int[4, 2] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } };

        private Dictionary<int, CharacterBase> characters = new Dictionary<int, CharacterBase>();
        private ISet<int> robotUnderController = new HashSet<int>();
        private ISet<int> deathList = new HashSet<int>();

        TPSCameraController TPSController = null;
        FightController fightController = null;

        public UIType<float> playerHP = new UIType<float>(UICacheKeys.PLAYER_HP, 1);

        private int userID = -1;             // 当前用户的 userID
        private int roomID = -1;             // 当前用户所在房间的ID

        GameObject mainCamera;

        private UIType<int> totalKillCount = new UIType<int>(UICacheKeys.TOTAL_KILL, 0);
        private UIType<int> totalPlayerCount = new UIType<int>(UICacheKeys.TOTAL_PLAYER, 0);

        public PlayersController(int userID, int roomID)
        {
            Clear();

            this.userID = userID;
            this.roomID = roomID;

            fightController = new FightController(roomID, userID);

            InitCamera();

            LoadMainPlayer();

            MonoMgr.Instance.AddUpdateEvent(Update);

            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_FIGHT_SYSTEM_SERVICE, CheckerPlayersHP);
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_QUERY_BORN_POINT_SERVICE, QueryRobotBornPointCallback);
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_SOLVE_WEAPONS_SERVICE, ChangedWeaponCallback);
            NetworkMgr.Instance.AddMsgListener(ServiceID.GAME_MGR_NEW_WEAPON_SERVICE, GetNewWeaponCallback);

            EventMgr.Instance.AddEventListener(EventName.PLAYER_OUT, PlayerOut);
            EventMgr.Instance.AddEventListener(EventName.MAIN_PLAYER_ACTION, UpdateMainPlayer);
            EventMgr.Instance.AddEventListener(EventName.PLAYER_ATTACKED, PlayerAttacked);
            EventMgr.Instance.AddEventListener(EventName.PLAYER_DEATH, PlayerDeath);
            EventMgr.Instance.AddEventListener(EventName.NEUTRAL_ROBOT_CHOOSE_GOAL, NeutralRobotChooseGoal);

            EventMgr.Instance.AddEventListener(EventName.PLAYER_KNIFE_ATTACK, MainPlayerKnifeAttack);
            EventMgr.Instance.AddEventListener(EventName.ENV_BALL_ATTACK, EvnBallAttack);
            EventMgr.Instance.AddEventListener(EventName.ENV_FREEZE_BEGIN, FreezeCheckBegin);
            EventMgr.Instance.AddEventListener(EventName.FIND_ROAD, RobotFindRoad);
        }

        public void Destroy()
        {
            MonoMgr.Instance.DelUpdateEvent(Update);

            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_FIGHT_SYSTEM_SERVICE, CheckerPlayersHP);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_QUERY_BORN_POINT_SERVICE, QueryRobotBornPointCallback);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_SOLVE_WEAPONS_SERVICE, ChangedWeaponCallback);
            NetworkMgr.Instance.RemoveMsgListener(ServiceID.GAME_MGR_NEW_WEAPON_SERVICE, GetNewWeaponCallback);

            EventMgr.Instance.DelEventListener(EventName.PLAYER_OUT, PlayerOut);
            EventMgr.Instance.DelEventListener(EventName.MAIN_PLAYER_ACTION, UpdateMainPlayer);
            EventMgr.Instance.DelEventListener(EventName.PLAYER_ATTACKED, PlayerAttacked);
            EventMgr.Instance.DelEventListener(EventName.PLAYER_DEATH, PlayerDeath);
            EventMgr.Instance.DelEventListener(EventName.NEUTRAL_ROBOT_CHOOSE_GOAL, NeutralRobotChooseGoal);

            EventMgr.Instance.DelEventListener(EventName.PLAYER_KNIFE_ATTACK, MainPlayerKnifeAttack);
            EventMgr.Instance.DelEventListener(EventName.ENV_BALL_ATTACK, EvnBallAttack);
            EventMgr.Instance.DelEventListener(EventName.ENV_FREEZE_BEGIN, FreezeCheckBegin);
            EventMgr.Instance.DelEventListener(EventName.FIND_ROAD, RobotFindRoad);

            fightController.Destroy();
        }

        public Vector3 FindNearestGoal(Vector3 nowPos)
        {
            Vector3 retPos = Vector3.zero;
            double minDis = 1e18;
            foreach (KeyValuePair<int, CharacterBase> keyValue in characters)
            {
                if (keyValue.Key < 0)
                {
                    continue;
                }
                Soldier soldier = keyValue.Value as Soldier;
                double dis = Vector3.Distance(soldier.GetTransform().position, nowPos);
                if (dis < minDis)
                {
                    minDis = dis;
                    retPos = soldier.GetTransform().position;
                }
            }
            return retPos;
        }

        public void RobotFindRoad(object obj)
        {
            int robotID = (int)obj;
            if (!characters.ContainsKey(robotID))
            {
                return;
            }
            Soldier robot = characters[robotID] as Soldier;

            Vector3 goalSoldier = FindNearestGoal(robot.GetTransform().position);

            if (MemeryCacheMgr.Instance.Get(DTSKeys.GAME_MAP) is GameMapController.GameMap gameMap)
            {
                Vector3 nowPos = robot.GetTransform().position;

                Vector3 retPos = FindNearestGoal(nowPos);

                //int rnd = UnityEngine.Random.Range(0, 4);

                //nowPos.x += Next[rnd, 0] * 10;
                //nowPos.z += Next[rnd, 1] * 10;

                EventMgr.Instance.EventTrigger(EventName.SET_CARROT + "#" + robot.GetUserID().ToString(), retPos);
            }
        }

        public void GetNewWeaponCallback(Message msg)
        {
            GameMgrRouter.NewWeaponResponse res = GameMgrRouter.NewWeaponRequestCallback(msg);
            if (res.ret != 0)
            {
                return;
            }
            if (!characters.ContainsKey(res.user_id))
            {
                return;
            }
            Soldier soldier = characters[res.user_id] as Soldier;
            soldier.SetWeapon(WeaponBase.ReflectionCreator(WeaponBoxBase.weaponTypeDict[WeaponBoxBase.IntToWeaponType[res.w_type]]), res.w_pos);
        }

        public void ChangedWeaponCallback(Message msg)
        {
            GameMgrRouter.SolveWeaponsResponse res = GameMgrRouter.SolveWeaponsRequestCallback(msg);
            if (res.ret != 0)
            {
                return;
            }
            if (!characters.ContainsKey(res.user_id))
            {
                return;
            }
            Soldier soldier = characters[res.user_id] as Soldier;
            soldier.bag.ChangeNowUsedWeapon(res.wid);
            soldier.FreshWeapon(res.wid);
        }

        public void SoldierFreezeCheck(FreezeMsg msg, Soldier soldier)
        {
            if (Vector3.Distance(msg.position, soldier.GetTransform().position) < msg.radius)
            {
                soldier.Freeze();
                MonoMgr.Instance.StartDelayEvent(FightController.FreezeLimit, () =>
                {
                    if (characters.ContainsKey(soldier.GetUserID()))
                    {
                        (characters[soldier.GetUserID()] as Soldier).UnFreeze();
                    }
                });
            }
        }

        public void FreezeCheckBegin(object obj)
        {
            if (obj is FreezeMsg msg)
            {
                foreach (KeyValuePair<int, CharacterBase> keyValue in characters)
                {
                    Soldier soldier = keyValue.Value as Soldier;
                    SoldierFreezeCheck(msg, soldier);
                }
            }
        }

        public void EvnBallAttack(object obj)
        {
            if (obj is GameObject ball)
            {
                foreach (KeyValuePair<int, CharacterBase> keyValue in characters)
                {
                    if (keyValue.Key > 0 && keyValue.Key != userID || keyValue.Key < 0 && !robotUnderController.Contains(keyValue.Key))
                    {
                        continue;
                    }
                    Soldier soldier = keyValue.Value as Soldier;
                    if (Vector3.Distance(ball.transform.position, soldier.GetTransform().position) * 2f < ball.transform.localScale.x)
                    {
                        EventMgr.Instance.EventTrigger(EventName.PLAYER_ATTACKED, new RoleHurtEvent(-1, soldier.GetUserID(), 3));
                    }
                }
            }
        }

        public void MainPlayerKnifeAttack(object obj)
        {
            int hp = (int)obj;
                
            if (!characters.ContainsKey(userID))
            {
                return;
            }

            Soldier mainPlayer = characters[userID] as Soldier;
            foreach (KeyValuePair<int, CharacterBase> keyValue in characters)
            {
                if (keyValue.Key == userID)
                {
                    continue;
                }
                Soldier soldier = keyValue.Value as Soldier;
                if (Vector3.Distance(mainPlayer.GetTransform().position, soldier.GetTransform().position) < DTSKeys.KNIFE_ATTACK_DISTANCE)
                {
                    EventMgr.Instance.EventTrigger(EventName.PLAYER_ATTACKED, new RoleHurtEvent(userID, soldier.GetUserID(), hp));
                }
            }
        }

        public void AddRobot(int robotID)
        {
            robotUnderController.Add(robotID);
        }

        public void NeutralRobotChooseGoal(object obj)
        {
            int robotID = (int)obj;

            if (!characters.ContainsKey(robotID))
            {
                return;
            }

            Soldier robot = characters[robotID] as Soldier;

            foreach (KeyValuePair<int, CharacterBase> keyValue in characters)
            {
                if (keyValue.Key == robotID)
                {
                    continue;
                }
                Soldier soldier = keyValue.Value as Soldier;

                if (soldier.GetUserID() > 0 && Vector3.Distance(robot.GetTransform().position, soldier.GetTransform().position) < 20)
                {
                    EventMgr.Instance.EventTrigger(EventName.ROBOT_ATTACK_EVENT, new RobotAttackEvent(robotID, soldier.GetGameObject()));
                }
            }
        }

        public bool IsUnderController(int robotID)
        {
            return robotUnderController.Contains(robotID);
        }

        private void CreateHpPack(Vector3 pos)
        {
            ResourceMgr.Instance.LoadAsync<GameObject>("model/weapon_box/HP", (obj) =>
            {
                obj.name = "hp";
                pos.y += 1;
                obj.transform.position = pos;
            });
        }

        private void PlayerDeath(object obj)
        {
            try
            {
                int userID = (int) obj;
                deathList.Add(userID);
                if (userID == this.userID)
                {
                    EventMgr.Instance.EventTrigger(EventName.GAME_OVER, new GameResult(false));
                    characters.Remove(userID);
                    return;
                }

                if (characters.ContainsKey(userID))
                {
                    Soldier soldier = characters[userID] as Soldier;
                    MonoMgr.Instance.StartDelayEvent(3000, () =>
                    {
                        if (soldier != null)
                        {
                            CreateHpPack(soldier.GetTransform().position);
                            soldier.Destory();
                        }
                    });
                    characters.Remove(userID);
                }

            }
            catch (Exception ex) { Debug.Log(ex.ToString()); }
        }

        private void AttackPlayer(int userID, int hp)
        {
            if (characters.ContainsKey(userID))
            {
                characters[userID].SetHp(hp);
                if (characters[userID].Death())
                {
                    characters[userID].GoToDeath();
                    if (characters.Count == 1 && userID != this.userID)
                    {
                        EventMgr.Instance.EventTrigger(EventName.GAME_OVER, new GameResult(true));
                    }
                }
                if (userID == this.userID)
                {
                    playerHP.val = characters[userID].GetHP();
                }
            }
        }

        private void CheckerPlayersHP(Message msg)
        {
            GameMgrRouter.FightSystemResponse res = GameMgrRouter.FightSystemRequestCallback(msg);
            if (res.opt.Equals(FightSystemKeys.QUERY_PLAYERS_HP))
            {
                // res.msg 格式为 user_id|HP#user_id|HP
                string []player_hp = res.msg.Split('#');
                for (int i = 0; i < player_hp.Length; i++)
                {
                    string []tmp = player_hp[i].Split('|');
                    try
                    {
                        int userID = Convert.ToInt32(tmp[0]);
                        int hp = Convert.ToInt32(tmp[1]);
                        AttackPlayer(userID, hp);
                    }
                    catch (Exception) { }
                }
            }
            else if (res.opt.Equals(FightSystemKeys.ATTACKED))
            {
                try
                {
                    string[] ss = res.msg.Split('#');
                    int userID = Convert.ToInt32(ss[0]);
                    int kill = Convert.ToInt32(ss[1]);
                    
                    if (userID == this.userID)
                    {
                        totalKillCount.val = kill;
                    }
                }
                catch (Exception) { }
            }
        }

        public void PlayerAttacked(object obj)
        {
            try
            {

                if (obj is RoleHurtEvent roleEvent && characters.ContainsKey(roleEvent.hittedUserID))
                {
                    // [1] 直接扣血
                    // characters[roleEvent.userID].Hit(roleEvent.hurtHP);
                    // [2] 服务端决定扣血 回调 CheckerPlayersHP
                    //Debug.Log(roleEvent.attackUserID + " attack " + roleEvent.hittedUserID + " hp " + roleEvent.hurtHP);
                    GameMgrRouter.FightSystemRequestCall(FightSystemKeys.ATTACKED, roomID, string.Format("{0},{1},{2}", 
                        roleEvent.attackUserID, roleEvent.hittedUserID, roleEvent.hurtHP));
                }
            }
            catch (Exception) { }
        }

        private void UpdateMainPlayer(object obj)
        {
            if (obj is PlayerAction action)
            {
                UpdateOperation(userID, action);
            }
        }

        public void UpdateOperation(int userID, PlayerAction playerAction)
        {
            if (characters.ContainsKey(userID))
            {
                characters[userID].UpdateOperation(playerAction);
            }
        }

        private void PlayerOut(object obj)
        {
            try
            {
                int user_id = (int) obj;
                if (characters.ContainsKey(user_id))
                {
                    characters[user_id].Destory();
                    DelCharactor(user_id);
                }
            }
            catch (Exception) { }
        }

        private Skill checkPlayers = new Skill("checker", 100);

        void Update()
        {
            if (checkPlayers.CheckAndRun())
            {
                GameMgrRouter.FightSystemRequestCall(FightSystemKeys.QUERY_PLAYERS_HP, roomID, "");
            }
            
            totalPlayerCount.val = characters.Count;

        }

        public CharacterBase GetUser(int userID)
        {
            if (characters.ContainsKey(userID))
            {
                return characters[userID];
            }
            return null;
        }

        private void InitCamera()
        {
            mainCamera = GameObject.Find("Main Camera");

            if (mainCamera == null)
            {
                ObjectPoolMgr.Instance.LoadGameObject("Main Camera", (camera) => {
                    mainCamera = camera;
                    camera.name = "Main Camera";
                    TPSController = camera.AddComponent<TPSCameraController>();
                });
            }
            else
            {
                TPSController = mainCamera.GetComponent<TPSCameraController>();
            }
        }

        public void LoadRobot(int robotID, int born)
        {
            // 加载一个机器人
            ResourceMgr.Instance.LoadAsync<GameObject>("model/PlayerA", (obj) =>
            {
                if (obj == null)
                {
                    Debug.Log("Player load fail");
                    return;
                }
                obj.name = "PlayerA#" + Convert.ToString(robotID);

                GameObject bornPoint = MemeryCacheMgr.Instance.Get(DTSKeys.ROBOT_BORN + "#" + born.ToString()) as GameObject;
                
                if (bornPoint == null)
                {
                    GameObject.Destroy(obj);
                    return;
                }

                obj.transform.position = bornPoint.transform.position;
                
                Soldier soldier = new Soldier(obj,
                    new AnimatorHandler(obj.transform.GetComponent<Animator>() as Animator),
                    SoldierStateBase.soldierStateWalk,
                    CharacterType.COMPUTER,
                    robotID,
                    roomID);

                ComputerController computerController = obj.AddComponent<ComputerController>();
                computerController.soldier = soldier;

                if (robotUnderController.Contains(robotID))
                {
                    computerController.controllerMode = ComputerController.ControllerMode.SELF;
                }
                else
                {
                    computerController.controllerMode = ComputerController.ControllerMode.NETWORK;
                }

                AddCharactor(robotID, soldier);
            });
        }

        public void LoadMainPlayer()
        {
            // 加载当前客户端控制的角色，命名为 PlayerA
            ResourceMgr.Instance.LoadAsync<GameObject>("model/PlayerA", (obj) =>
            {
                if (obj == null)
                {
                    Debug.Log("Player load fail");
                    return;
                }
                obj.name = "PlayerA";

                int x = UnityEngine.Random.Range(10, (DTSKeys.MAP_ROW - 1) * 10);
                int z = UnityEngine.Random.Range(10, (DTSKeys.MAP_COL - 1) * 10);

                if (roomID == 0)
                {
                    obj.transform.position = mainCamera.transform.position;
                }
                else
                {
                    obj.transform.position = new Vector3(x, 10, z);
                }
                
                Soldier soldier = new Soldier(obj,
                    new AnimatorHandler(obj.transform.GetComponent<Animator>() as Animator),
                    SoldierStateBase.soldierStateWalk,
                    CharacterType.PLAYER,
                    userID,
                    roomID);

                SoldierController soldierController = obj.AddComponent<SoldierController>();
                soldierController.soldier = soldier;

                AddCharactor(userID, soldier);

                TPSController.Init();
            });
        }

        private void QueryRobotBornPointCallback(Message msg)
        {
            GameMgrRouter.QueryBornPointResponse res = GameMgrRouter.QueryBornPointRequestCallback(msg);
            if (res.ret == 0)
            {
                LoadRobot(res.user_id, res.born);
            }
        }

        private void LoadOtherPlayer(int otherUserID)    
        {
            if (otherUserID < 0)
            {
                GameMgrRouter.QueryBornPointRequestCall(otherUserID, roomID);   // call back QueryRobotBornPointCallback
                return ;
            }

            // 加载其他玩家控制的角色，命名为 PlayerA#用户ID
            ResourceMgr.Instance.LoadAsync<GameObject>("model/PlayerA", (obj) =>
            {
                if (obj == null)
                {
                    Debug.Log("Player load fail");
                    return;
                }

                Debug.Log("other_user_id" + otherUserID);
                
                obj.name = "PlayerA#" + Convert.ToString(otherUserID);
                obj.transform.position = new Vector3(0, 20, 0);

                WeaponBase weapon = new WeaponAK47();

                Soldier soldier = new Soldier(obj,
                    new AnimatorHandler(obj.transform.GetComponent<Animator>() as Animator),
                    SoldierStateBase.soldierStateWalk,
                    CharacterType.OTHER_PLAYER,
                    otherUserID,
                    roomID);

                OtherPlayerController soldierController = obj.AddComponent<OtherPlayerController>();
                soldierController.soldier = soldier;

                AddCharactor(otherUserID, soldier);
            });
        }

        public void CompareUserSet(string [] userIDList)
        {
            for (int i = 0; i < userIDList.Length; i++ )
            {
                try
                {
                    int userID = Convert.ToInt32(userIDList[i]);

                    if (deathList.Contains(userID))
                    {
                        continue;
                    }
                    
                    if (userID != this.userID && !characters.ContainsKey(userID))
                    {
                        LoadOtherPlayer(userID);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("CompareUserSet error: " + ex.ToString());
                }
            }
        }
        
        private void AddCharactor(int userID, CharacterBase character)
        {
            if (!characters.ContainsKey(userID))
            {
                characters.Add(userID, character);
            }
            else
            {
                characters[userID] = character;
            }
        }

        private void DelCharactor(int userID)
        {
            if (characters.ContainsKey(userID))
            {
                characters.Remove(userID);
            }
        }

        private void Clear()
        {
            characters.Clear();
        }
    }
}
