using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;
using CWLEngine.Core.Network;
using CWLEngine.GameImpl.Base;
using CWLEngine.GameImpl.Entity;
using CWLEngine.GameImpl.Network;
using CWLEngine.GameImpl.UI;
using CWLEngine.GameImpl.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CWLEngine.GameImpl.Controller
{
    public class GameMapController
    {
        public class GameMap
        {
            public int row, col;
            public int[][] gameMap;     // 1 是路  0 是墙

            public static int ROAD = 1;
            public static int WALL = 0;

            public GameMap(int row, int col)
            {
                this.row = row;
                this.col = col;
                gameMap = new int[row + 2][];

                for (int i = 0; i < row + 2; i++ )
                {
                    gameMap[i] = new int[col + 2];
                }
            }

            public bool Inside(int x, int y)
            {
                return x >= 0 && y >= 0 && x <= row + 1 && y <= row + 1;
            }

            readonly int[,] Next = new int[4, 2] { { -1, 0 }, { 1, 0 }, { 0, 1 }, { 0, -1 } };

            public bool DeathPlace(int x, int y)
            {
                if (!Inside(x, y))
                {
                    return false;
                }
                int count = 0;

                for (int dir = 0; dir < 4; dir++ )
                {
                    int nextX = x + Next[dir, 0];
                    int nextY = y + Next[dir, 1];
                    if (!Inside(nextX, nextY))
                    {
                        continue;
                    }
                    if (gameMap[nextX][nextY] == 0)
                    {
                        count += 1;
                    }
                }
                return count >= 3;
            }
        }
        GameMap gameMap;
        GameObject ball = null;
        float beginTime = 0;

        public GameMapController()
        {
            MonoMgr.Instance.AddUpdateEvent(BallExtend);
        }

        private static List<int> mapTemple1 = new List<int>()
        {
            0, 45054, 41642, 65194, 43520, 43774, 32896, 64170, 41642, 48126, 8834, 64170, 8234, 64190, 8866, 65530, 0
        };

        private readonly int MAP_ROW = DTSKeys.MAP_ROW;
        private readonly int MAP_COL = DTSKeys.MAP_COL;

        public string []modelList = new string[]
        {
            "model/robot/robot_life",

        };

        public void CreateStatic()
        {
            beginTime = Time.time;
            MonoMgr.Instance.StartCoroutine(CreatorMapStatic());
        }

        public void Create()
        {
            beginTime = Time.time;
            MonoMgr.Instance.StartCoroutine(CreatorMap());
        }

        void RandomRobot(int posx, int posz, ref int robotCount, ref int weaponCount, ref int createCount, GameObject floder)
        {
            if (createCount % 4 == 0)
            {
                if (gameMap.DeathPlace(posx, posz) && robotCount < DTSKeys.MAX_ROBOT_LIMIT)
                {
                    int robotKey = robotCount;
                    ResourceMgr.Instance.LoadAsync<GameObject>("model/robot/robot_life", (obj) =>
                    {
                        obj.transform.SetParent(floder.transform);
                        obj.name = "robot";
                        obj.transform.position = new Vector3(posx * 10, 10, posz * 10);
                        RobotCreator creator = obj.GetComponent<RobotCreator>();
                        creator.CreateRobot(robotKey);
                    });
                    robotCount += 1;
                }
            }
            else if (createCount % 4 == 1 && gameMap.DeathPlace(posx, posz) && weaponCount < DTSKeys.MAX_WEAPON_BOX_LIMIT)
            {
                string name = WeaponBoxBase.weaponModelName[weaponCount % WeaponBoxBase.weaponModelName.Count];
                
                ResourceMgr.Instance.LoadAsync<GameObject>("model/weapon_box/" + name, (obj) =>
                {
                    obj.transform.SetParent(floder.transform);
                    obj.name = "weapon_box";
                    obj.transform.position = new Vector3(posx * 10, 0.5f, posz * 10);
                });
                weaponCount += 1;
            }
        }

        public void Stop()
        {
            MonoMgr.Instance.DelUpdateEvent(BallExtend);
        }

        void ShowMap()
        {
            GameObject floder = new GameObject("cube_floder");

            int robotCount = 0;
            int weaponCount = 0;
            int createCount = 0;

            List<int> mapBackUp = new List<int>();

            for (int i = 0; i < gameMap.row + 2; i++ )
            {
                int number = 0;
                for (int j = 0; j < gameMap.col + 2; j++)
                {
                    int posx = i;
                    int posz = j;
                    int val = gameMap.gameMap[i][j];
                    number ^= (val << j);
                    ResourceMgr.Instance.LoadAsync<GameObject>("model/env/Game/Cube", (obj) =>
                    {
                        obj.transform.SetParent(floder.transform);
                        obj.name = "cube";
                        obj.transform.localScale = Vector3.one * 10;
                        obj.transform.position = new Vector3(posx * 10, -val * 5, posz * 10);
                    });
                    
                    // TODO DEL
                    //int x = i;
                    //int y = j;
                    //ResourceMgr.Instance.LoadAsync<GameObject>("model/env/Game/Cube", (obj) =>
                    //{
                    //    obj.name = "cc" + x.ToString() + " " + y.ToString();
                    //    obj.transform.localScale = Vector3.one;
                    //    obj.transform.position = new Vector3(x * 10, 10, y * 10);
                    //});

                    if (val == 1)
                    {
                        RandomRobot(posx, posz, ref robotCount, ref weaponCount, ref createCount, floder);
                        createCount++;
                    }
                }
                mapBackUp.Add(number);
            }

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < mapBackUp.Count; i++)
            {
                stringBuilder.Append(mapBackUp[i].ToString() + ", ");
            }
            Debug.Log(stringBuilder.ToString());

            ResourceMgr.Instance.LoadAsync<GameObject>("model/env/Game/ball", (obj) =>
            {
                obj.transform.SetParent(floder.transform);
                obj.transform.position = new Vector3(0, 0, 0);
                obj.transform.localScale = new Vector3(0, 0, 0);
                ball = obj;
            });
        }

        private Skill ballAttack = new Skill("ball", 1000);

        void BallExtend()
        {
            if (ball == null)
            {
                return;
            }
            float extend = Time.time - beginTime;
            float x = 2f;
            ball.transform.localScale = new Vector3(extend * x, extend * x, extend * x);

            if (ballAttack.CheckAndRun())
            {
                EventMgr.Instance.EventTrigger(EventName.ENV_BALL_ATTACK, ball);
            }
        }

        IEnumerator CreatorMapStatic()
        {
            Stack<KeyValuePair<int, int>> stk = new Stack<KeyValuePair<int, int>>();
            gameMap = new GameMap(MAP_ROW, MAP_COL);
            
            for (int i = 0; i < MAP_ROW + 2; i++ )
            {
                int number = mapTemple1[i];
                for (int j = 0; j < MAP_COL + 2; j++ )
                {
                    gameMap.gameMap[i][j] = (number >> j) & 1;
                }
            }

            MemeryCacheMgr.Instance.Set(DTSKeys.GAME_MAP, gameMap);

            ShowMap();

            yield return null;
        }

        IEnumerator CreatorMap()
        {
            Stack<KeyValuePair<int, int>> stk = new Stack<KeyValuePair<int, int>>();
            gameMap = new GameMap(MAP_ROW, MAP_COL);

            int [ , ]Next = new int[4, 2]{ {-1, 0}, {1, 0},{0, 1}, {0, -1}};

            stk.Push(new KeyValuePair<int, int>(1, 1));

            while (stk.Count > 0)
            {
                KeyValuePair<int, int> top = stk.Pop();
                int nowX = top.Key;
                int nowY = top.Value;

                for (int dir = 0; dir < 4; dir++)
                {
                    int rnd = UnityEngine.Random.Range(0, 4);
                    int tmpX = Next[0, 0];
                    int tmpY = Next[0, 1];
                    Next[0, 0] = Next[rnd, 0];
                    Next[0, 1] = Next[rnd, 1];
                    Next[rnd, 0] = tmpX;
                    Next[rnd, 1] = tmpY;
                }

                gameMap.gameMap[nowX][nowY] = 1;

                for (int dir = 0; dir < 4; dir++ )
                {
                    int goalX = nowX + Next[dir, 0];
                    int goalY = nowY + Next[dir, 1];
                    int nextX = nowX + Next[dir, 0] * 2;
                    int nextY = nowY + Next[dir, 1] * 2;

                    if (!gameMap.Inside(nextX, nextY))
                    {
                        continue;
                    }
                    
                    if (gameMap.gameMap[nextX][nextY] == 0)
                    {
                        gameMap.gameMap[goalX][goalY] = 1;
                        gameMap.gameMap[nextX][nextY] = 1;
                        stk.Push(new KeyValuePair<int, int>(nextX, nextY));
                    }
                }
            }

            ShowMap();
            
            yield return null;
        }

    }
}
