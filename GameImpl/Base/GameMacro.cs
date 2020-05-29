using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CWLEngine.Core.Base;
using CWLEngine.Core.Manager;

namespace CWLEngine.GameImpl.Base
{
    public class NetworkMacro
    {
        public const string LOCAL_IP = "127.0.0.1"; // "117.78.5.122";
        public const int LOCAL_PORT = 7736;
        public const string NETWORK_IP = "117.78.5.122";
        public const int NETWORK_PORT = 7736;
    }

    // 包含所有游戏场景的路径
    public class ScenePath
    {
        public const string LOGIN = "Scenes/UI/login";
        public const string LOADING = "Scenes/UI/loading";
        public const string LEVEL_0 = "Scenes/Game/Level_0";
        public const string LEVEL_1 = "Scenes/Game/Level_1";
        public const string LEVEL_00 = "Scenes/Game/Level_00";
        public const string LEVEL_INIT = "Scenes/Game/Level_Init";
        public const string LEVEL_EMPTY = "Scenes/Game/Level_Empty";
    }

    public class ServiceID
    {
        public const int USER_NETWORK_TEST_SERVICE = 666;
        public const int USER_LOGIN_SERVICE = 1001;
        public const int USER_REGISTER_SERVICE = 1002;
        public const int USER_CHANGE_PASSWORD_SERVICE = 1003;
        public const int USER_USER_LEVEL_SERVICE = 1004;

        public const int ROOM_ENTER_ROOM_SERVICE = 1010;
        public const int ROOM_QUERY_ROOM_USERS_SERVICE = 1011;
        public const int ROOM_QUERY_USER_BELONGED_ROOM_SERVICE = 1012;

        public const int SYNCHRONIZATION_QUERY_USER_TRANSFORM_SERVICE = 1020;
        public const int SYNCHRONIZATION_REPORT_TRANSFORM_SERVICE = 1021;
        public const int SYNCHRONIZATION_HEART_BEAT_SERVICE = 1022;
        public const int SYNCHRONIZATION_REPORT_ACTION_SERVICE = 1023;
        public const int SYNCHRONIZATION_QUERY_ACTION_SERVICE = 1024;
        public const int SYNCHRONIZATION_REPORT_ATTACK_SERVICE = 1025;
        public const int SYNCHRONIZATION_QUERY_ATTACK_SERVICE = 1026;

        public const int GAME_MGR_PLAY_ALONE_SERVICE = 1030;
        public const int GAME_MGR_PLAY_WITH_OTHERS_SERVICE = 1031;
        public const int GAME_MGR_QUERY_MATCHING_RESULT_SERVICE = 1032;
        public const int GAME_MGR_PLAYER_EVENT_SERVICE = 1033;
        public const int GAME_MGR_FIGHT_SYSTEM_SERVICE = 1034;
        public const int GAME_MGR_REGISTER_ROBOT_SERVICE = 1035;
        public const int GAME_MGR_QUERY_BORN_POINT_SERVICE = 1036;
        public const int GAME_MGR_SOLVE_WEAPONS_SERVICE = 1037;
        public const int GAME_MGR_AOE_FREEZE_SERVICE = 1038;
        public const int GAME_MGR_NEW_WEAPON_SERVICE = 1039;
        public const int GAME_MGR_ADD_HP_SERVICE = 1040;
    }

    public class NetworkFrequency
    {
        public const float SYNC_TRANSFORM_FREQUENT = 66; // ms
        public const float SYNC_ACTION_FREQUENT = 66;      // ms

        public const float HEART_BEAT = 1; // s
    }

    // 包含所有游戏场景的名字
    public class SceneName
    {
        public const string LOGIN = "Login";
        public const string LOADING = "Scenes/UI/loading";
        public const string LEVEL_0 = "Scenes/Game/Level_0";
        public const string LEVEL_1 = "Scenes/Game/Level_1";
    }
    
    public class UIPanelPath
    {
        public const string LOGIN = "login";
        public const string REGISTER = "register";
        public const string CHANGE_PASSWORD = "changepswd";
        public const string MAIN_MENU = "main_menu";
        public const string SETTING = "setting";
        public const string GAME_RUN = "game_run";

        public const string GAME_OPT = "game_opt_1";
        public const string RESULT_PANEL = "result_panel";
        public const string WARN_MESSAGE_BOX = "warn_box";
    }

    // 对应 Animation 中的一些 动画，参数名字
    public class DTSAnimation
    {
        // 用于 animation blend tree 2d 状态的两个参数名
        public const string BLEND_TREE_2D_PARAM_NAME_1 = "posx";
        public const string BLEND_TREE_2D_PARAM_NAME_2 = "posy";

        // 用于 animation 中的 某些行为的 触发器
        public const string SHOOT = "attack1";
        public const string SHOOT_BURST = "attack2";
        public const string RELOAD = "reload";

        // 用于 animation 中 武器种类的枚举，第一个是变量名
        public const string WEAPON_PARAM = "weapon";
        public const int ANIMATION_STATE_WEAPON_KNIFE = 1;
        public const int ANIMATION_STATE_WEAPON_INFANTRY = 2;
        public const int ANIMATION_STATE_WEAPON_HANDGUN = 3;
        public const int ANIMATION_STATE_WEAPON_HEAVY = 4;

        // 用于 animation 中 姿势(站立，蹲下...) 的枚举，第一个是变量名
        public const string POSE_PARAM = "state";
        public const int ANIMATION_STATE_STAND = 1;
        public const int ANIMATION_STATE_CROUCH = 2;
        
        public const string ANIMATION_STATE_STAND_STR = "ANIMATION_STATE_STAND_STR";
        public const string ANIMATION_STATE_CROUCH_STR = "ANIMATION_STATE_CROUCH_STR";
        public const string ANIMATION_STATE_DUMP_STR = "ANIMATION_STATE_DUMP_STR";
        public const string ANIMATION_STATE_DROP_DOWN_STR = "ANIMATION_STATE_DROP_DOWN_STR";
        // =====================================================================================
        // use knife
        public const string KNIFE_STAND = "knife_stand";
        public const string KNIFE_CROUCH = "knife_crouch";

        public const string KNIFE_DRUMP_1 = "knife_jump_1_start";
        public const string KNIFE_DRUMP_2 = "knife_jump_2_air";
        public const string KNIFE_DRUMP_3 = "knife_jump_3_land";

        public const string KNIFE_STAND_ATTACK1 = "knife_combat_attack_A";
        public const string KNIFE_STAND_ATTACK2 = "knife_combat_attack_A 0";    // 这个0是命名就那样，绝对不是手抖按上去的
        public const string KNIFE_CROUCH_ATTACK1 = "knife_crouch_attack_A";
        public const string KNIFE_CROUCH_ATTACK2 = "knife_crouch_attack_B";

        // =====================================================================================
        // use infantry
        public const string INFANTRY_STAND = "infantry_stand";
        public const string INFANTRY_CROUCH = "infantry_crouch";

        public const string INFANTRY_DRUMP_1 = "infantry_jump_1_start";
        public const string INFANTRY_DRUMP_2 = "infantry_jump_2_air";
        public const string INFANTRY_DRUMP_3 = "infantry_jump_3_land";

        public const string INFANTRY_STAND_ATTACK1 = "infantry_combat_shoot";
        public const string INFANTRY_STAND_ATTACK2 = "infantry_combat_shoot_burst";
        public const string INFANTRY_CROUCH_ATTACK1 = "infantry_crouch_shoot";
        public const string INFANTRY_CROUCH_ATTACK2 = "infantry_crouch_shoot_burst";
        public const string INFANTRY_RELOAD = "infantry_combat_reload";
        // =====================================================================================
        // use handgun
        public const string HANDGUN_STAND = "handgun_stand";
        public const string HANDGUN_CROUCH = "handgun_crouch";

        public const string HANDGUN_DRUMP_1 = "handgun_jump_1_start";
        public const string HANDGUN_DRUMP_2 = "handgun_jump_2_air";
        public const string HANDGUN_DRUMP_3 = "handgun_jump_3_land";

        public const string HANDGUN_STAND_ATTACK1 = "handgun_combat_shoot";
        public const string HANDGUN_STAND_ATTACK2 = "handgun_combat_shoot_burst";
        public const string HANDGUN_CROUCH_ATTACK1 = "handgun_crouch_shoot";
        public const string HANDGUN_CROUCH_ATTACK2 = "handgun_crouch_shoot_burst";
        public const string HANDGUN_RELOAD = "handgun_combat_reload";

        // =====================================================================================
        // use heavy
        public const string HEAVY_STAND = "heavy_stand";
        public const string HEAVY_CROUCH = "heavy_crouch";

        public const string HEAVY_DRUMP_1 = "heavy_jump_1_start";
        public const string HEAVY_DRUMP_2 = "heavy_jump_2_air";
        public const string HEAVY_DRUMP_3 = "heavy_jump_3_land";

        public const string HEAVY_STAND_ATTACK1 = "heavy_combat_shoot";
        public const string HEAVY_CROUCH_ATTACK1 = "heavy_crouch_shoot";
        public const string HEAVY_RELOAD = "heavy_combat_reload";

        // ====================================================================================
        // death
        public const string DEATH_1 = "infantry_death_E";
                                      
    }

    // EContexParam ContexParam.Params 顺序不能变
    // EContexParam 第一个 VIEW
    // ContexParam.Params 第一个也是 VIEW
    /**
     * 
     *  StateContex.cs 
        public string Get(EContexParam id)
        {
            return ContexParam.Params[ (int) id];
        }
         */
    public enum EContexParam
    {
        VIEW,
        SCREAM_X,
        SCREAM_Y,

        MOVE_HV,
        HORIZONTAL,
        VERTICAL,

        ANIMATION_BLEND_TREE_2D_PARAM_NAME_1,
        ANIMATION_BLEND_TREE_2D_PARAM_NAME_2,
        
        SHOOT,
        SHOOT_BURST,
        SHOOT_LINE, 
        RELOAD,
        
        BEGIN_RUN,
        END_RUN,
        BEGIN_CROUCH,
        END_CROUCH,
        
        DUMP,
        PRE_STATE,
        CHANGE_WAEPON,

        USE_OLD_WEAPON,
    }

    public class ContexParam
    {
        public static string[] Params = new string[]{
            VIEW, SCREAM_X, SCREAM_Y,
            MOVE_HV, HORIZONTAL, VERTICAL,
            ANIMATION_BLEND_TREE_2D_PARAM_NAME_1, ANIMATION_BLEND_TREE_2D_PARAM_NAME_2,
            SHOOT, SHOOT_BURST, SHOOT_LINE, RELOAD,
            BEGIN_RUN, END_RUN, BEGIN_CROUCH, END_CROUCH,
            DUMP, PRE_STATE, CHANGE_WAEPON,
            USE_OLD_WEAPON,
        };

        public const string VIEW = "VIEW";
        public const string SCREAM_X = "SCREAM_X";
        public const string SCREAM_Y = "SCREAM_Y";

        public const string MOVE_HV = "MOVE";
        public const string HORIZONTAL = "Horizontal";
        public const string VERTICAL = "Vertical";

        public const string ANIMATION_BLEND_TREE_2D_PARAM_NAME_1 = "posx";
        public const string ANIMATION_BLEND_TREE_2D_PARAM_NAME_2 = "posy";
        // 开火
        public const string SHOOT = "SHOOT";
        public const string SHOOT_BURST = "SHOOT_BURST";
        public const string SHOOT_LINE = "SHOOT_LINE";
        public const string RELOAD = "RELOAD";
        
        // 跑
        public const string BEGIN_RUN = "BR";
        public const string END_RUN = "ER";

        // 蹲
        public const string BEGIN_CROUCH = "BC";
        public const string END_CROUCH = "EC";

        // 跳
        public const string DUMP = "DUNP";

        // 存储上一个状态
        public const string PRE_STATE = "PRE_STATE";

        public const string CHANGE_WAEPON = "CHANGE_WEAPON";

        public const string USE_OLD_WEAPON = "USE_OLD_WEAPON";
    }

    public class GameObjectName
    {
        public const string MainCameraName = "Main Camera";
    }

    public class IKPosition
    {
        public const string IKLeftHand = "IKLeftHand";
        public const string IKRightHand = "IKRightHand";
    }

    public class DTSKeys
    {
        public const string GAME_MAP = "GAME_MAP";
        public const string DROP_DOWN = "DROP_DOWN";
        public const string IS_IN_INIT_ROOM = "IS_IN_INIT_ROOM";

        public const string MATCHING = "MATCHING";
        public const string USER_ID = "user_id";
        public const string ROOM_ID = "room_id";

        public const string ROBOT_BORN = "robot_born";

        public const int MAP_ROW = 15;
        public const int MAP_COL = 15;

        public const int MAX_ROBOT_LIMIT = 5;
        public const int MAX_WEAPON_BOX_LIMIT = 30;

        public const string ROBOT_BORN_LIST = "ROBOT_BORN_LIST";

        public const float MODEL_MIN_DISTANCE = 1.8f;
        public const float KNIFE_ATTACK_DISTANCE = 2f;
        public const float GUN_ATTACK_DISTANCE = 10f;
    }
    
    public class UICacheKeys
    {
        public const string EVE = "EVE";
        public const string NO_BIND = "NO_BIND";

        // 武器信息
        public const string BULLET_COUNT_FIRST = "BULLET_COUNT_FIRST";
        public const string BULLET_COUNT_SECOND = "BULLET_COUNT_SECOND";
        public const string HURT_ICON_SHOW = "HURT_ICON_SHOW";
        public const string CENTER_STAR_ICON_EXPAND = "CENTER_STAR_ICON_EXTENT";
        public const string WEAPON_NAME = "WEAPON_NAME";

        // 计分板
        public const string TOTAL_KILL = "TOTAL_KILL";
        public const string TOTAL_PLAYER = "TOTAL_PLAYER";
        public const string GAME_RESULT = "GAME_RESULT";

        // 提示框
        public const string BULLET_BOX_WARN_MESSAGE = "BULLET_BOX_WARN_MESSAGE";
        public const string WEAPON_BOX = "WEAPON_BOX";

        public const string MESSAGE_STRING = "MESSAGE_STRING";

        public const string BTN_MATCHING = "BTN_MATCHING";
        public const string PLAYER_HP = "PLAYER_HP";

    }

    public class EventName
    {
        // 玩家相关
        public const string PLAYER_OUT = "PLAYER_OUT";
        public const string MAIN_PLAYER_ACTION = "MAIN_PLAYER_ACTION";

        public const string PLAYER_ATTACKED = "PLAYER_ATTACKED";

        public const string SHOOT = "SHOOT";
        public const string SHOOT_BRUST = "SHOOT_BRUST";
        public const string RELOAD = "RELOAD";

        public const string BEGIN_RUN = "BEGIN_RUN";
        public const string END_RUN = "END_REN";

        public const string BEGIN_CROUCH = "BEGIN_CROUCH";
        public const string END_CROUCH = "END_CROUCH";
        
        public const string DUMP = "DUNP";

        public const string MOVE = "MOVE";
        public const string VIEW = "VIEW";

        public const string UPDATE_ACTION = "UPDATE_ACTION";

        public const string PLAYER_CHANGE_WEAPON = "PLAYER_CHANGE_WEAPON";
        
        // 游戏结果
        public const string PLAYER_DEATH = "PLAYER_DEATH";
        public const string GAME_OVER = "GAME_OVER";

        // 机器人相关
        public const string FIGHT_ROBOT_CONTROLLER = "FIGHT_ROBOT_CONTROLLER";
        
        public const string NEUTRAL_ROBOT_CHOOSE_GOAL = "NEUTRAL_ROBOT_MODE";
        public const string ROBOT_ATTACK_EVENT = "ROBOT_ATTACK_EVENT";
        public const string INIT_ALL_ROBOT = "INIT_ALL_ROBOT";

        public const string SET_CARROT = "SET_CARROT";
        public const string FIND_ROAD = "FIND_ROAD";

        // 主玩家攻击相关
        public const string PLAYER_KNIFE_ATTACK = "PLAYER_KNIFE_ATTACK";

        // 环境攻击
        public const string ENV_BALL_ATTACK = "ENV_BALL_ATTACK";

        public const string ATTACK_YUN = "ATTACK_YUN";

        public const string ENV_FREEZE_BEGIN = "ENV_FREEZE_BEGIN";

    }

    public class FightSystemKeys
    {
        public const string ATTACKED = "attacked";
        public const string QUERY_PLAYERS_HP = "query_players";
    }
}
