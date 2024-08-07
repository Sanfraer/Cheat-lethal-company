using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using GameNetcodeStuff;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering;
using UnityEngine.Windows;

namespace Lethal_Company
{
    [BepInPlugin ("Sanfraer", "null", "1.0")]
    public class CheatMain : MonoBehaviour
    {
        # region Variables
        //Menu
        private bool menuState = true;
        private bool PlayersMenu = false;      
        private bool TerminalInfo = false;
        //private bool TerminalHost = false;
        private bool menuTP = false;
        private bool spawnMenu = false;
        private bool flashlightMenu = false;


        public string menuMode = "Main";

        //Main
        private Rect rectMenu = new Rect(10f, 10f, 360f, 336f);
        private Rect dragMenu = new Rect(0f, 0f, 336f, 20f);
        //ListPlayer
        private Rect rectPlayersMenu = new Rect(400f, 100f, 500f, 475f);
        private Rect dragPlayersMenu = new Rect(0f, 0f, 336f, 20f);
        //Host menu
        private Rect rectHost = new Rect(250f, 575f, 325f, 550f);
        private Rect dragHost = new Rect(0f, 0f, 336f, 20f);
        //Display info
        private Rect rectTerminalInfo = new Rect(100f, 450f, 250f, 270f);
        private Rect dragTerminalInfo = new Rect(0f, 0f, 336f, 336f);
        //MenuTP
        private Rect rectMenuTP = new Rect(325, 455, 340f, 230f);
        private Rect dragMenuTP = new Rect(0f, 0f, 336f, 336f);
        //Spawn Menu
        private Rect rectSpawn = new Rect(350, 25, 336f, 336f);
        private Rect dragSpawn = new Rect(0f, 0f, 336f, 336f);
        //Flaslight Menu
        private Rect rectFlashlight = new Rect(500f, 55f, 235f, 170f);
        private Rect dragFlashlight = new Rect(0f, 0f, 336f, 336f);

        private GUILayoutOption[] buttonSize = new GUILayoutOption[] { GUILayout.Width(105f), GUILayout.Height(25) };

        //Variables
        public PlayerControllerB player;
        public NetworkBehaviour networkBehaviour;
        public ShipLights shipLights;
        public WalkieTalkie walkieTalkie;
        public Terminal terminal;
        public HangarShipDoor hangarShipDoor;
        public PlayerControllerB selectedPlayer;
        public DoorLock doorLock;
        public EnemyAI enemyAI;
        public StartMatchLever startMatch;
        public Landmine landmine;
        public StartOfRound startOf;
        public Rigidbody rigidbody;
        public CharacterController characterController;
        public TimeOfDay timeOf;
        public Shovel shovel;
        public FlashlightItem fls;

        //Array
        public PlayerControllerB[] Players;
        public GrabbableObject[] grabbableObjects;
        public EnemyAI[] EnemiesAI;

        //boolStatic
        private bool Stamina;
        private bool Health;
        private bool Grab;
        private bool shipLight;
        private bool Jump;
        private bool movSpeed;
        private bool jetpack;
        private bool shipDoor;
        private bool climbSpeed;
        private bool terminalDoors;
        private bool Battery;
        private bool ShovelHit;

        //F static
        private float characterControllers;

        private string creditInput = "0";

        public Vector2 spawnerScroller;

        #endregion

        void OnGUI()
        {
            GUI.color = Color.gray;

            if (menuState)
            {
                switch (menuMode)
                {
                    case "Main":
                        rectMenu = GUI.Window(0, rectMenu, (GUI.WindowFunction)Main, "SanWare - BETA");
                        break;
                    case "Players":
                        rectPlayersMenu = GUI.Window(1, rectPlayersMenu, (GUI.WindowFunction)PlayerMenuB, "Players");
                        break;
                    case "TerminalInfo":
                        rectTerminalInfo = GUI.Window(2, rectTerminalInfo, (GUI.WindowFunction)Terminal, "Terminal info");
                        break;
                    case "menuTP":
                        rectMenuTP = GUI.Window(4, rectMenuTP, (GUI.WindowFunction)menuTeleport, "Teleport menu");
                        break;
                    case "spawnMenu":
                        rectSpawn = GUI.Window(5, rectSpawn, (GUI.WindowFunction)Spawner, "Spawner");
                        break;
                    case "flashlightMenu":  
                        rectFlashlight = GUI.Window(6, rectFlashlight, Flashlight, "Flashlight Menu");
                        break;
                }
               /* if (TerminalHost)
                {
                    rectHost = GUI.Window(3, rectHost, (GUI.WindowFunction)HostTerminal, "Host Terminal");
                } */
            }
        }
    

        private void Main(int ID)
        {
            if (menuState)
            {
                GUI.color = Color.cyan;
                GUILayout.BeginHorizontal(GUI.skin.box, GUILayout.ExpandHeight(true));
                //1 Столбец
                GUILayout.BeginVertical();
                if (GUILayout.Button(MakeToggle("Stamina", Stamina), buttonSize))
                {
                    Stamina = !Stamina;

                    var SOR = FindObjectOfType<StartOfRound>();
                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;

                        if (Stamina)
                        {
                            localPlayer.sprintTime = 9999f; 
                            localPlayer.isExhausted = false; 
                        }
                        else
                        {
                            localPlayer.sprintTime = 13f;
                            localPlayer.isExhausted = true;
                        }
                    }
                }

                if (GUILayout.Button(MakeToggle("Health", Health), buttonSize))
                {
                    Health = !Health;
                    
                    var SOR = FindAnyObjectByType<StartOfRound>();
                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;
                        if (Health)
                        {
                            localPlayer.health = 9999999;
                        }
                        else
                        {
                            localPlayer.health = 100;
                        }
                    }
                }

                if (GUILayout.Button(MakeToggle("GrabDist", Grab), buttonSize))
                {
                    Grab = !Grab;

                    var SOR = FindAnyObjectByType<StartOfRound>();
                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;
                        if (Grab)
                        {
                            localPlayer.grabDistance = 20;
                        }
                        else
                        {
                            localPlayer.grabDistance = 5;
                        }
                    }
                }
                if (GUILayout.Button(MakeToggle("Jump", Jump), buttonSize))
                {
                    Jump = !Jump;

                    var SOR = FindObjectOfType<StartOfRound>();
                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;
                        if (Jump)
                        {
                            localPlayer.jumpForce = 25f;
                        }
                        else
                        {
                            localPlayer.jumpForce = 10f;
                        }
                    }
                }

                if (GUILayout.Button(MakeToggle("OneHit", ShovelHit), buttonSize))
                {
                    ShovelHit = !ShovelHit;

                    var SOR = FindAnyObjectByType<StartOfRound>();
                    if (SOR != null)
                    {
                        shovel = FindObjectOfType<Shovel>();
                        shovel.shovelHitForce = 999;
                    }
                    else
                    {
                        shovel.shovelHitForce = 1;
                    }
                }

                GUILayout.BeginArea(new Rect(25f, 250f, 185f, 100f)); 
                GUILayout.Label("Enter Credits:");
                creditInput = GUILayout.TextField(creditInput, GUILayout.Width(100f));
                if (GUILayout.Button("Add", GUILayout.Width(50f)))
                {
                    AddCredits();
                }
                GUILayout.EndArea(); 

                if (GUILayout.Button(MakeToggle("Speed", movSpeed), buttonSize))
                {
                    movSpeed = !movSpeed;

                    var SOR = FindObjectOfType<StartOfRound>();
                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;
                        if (movSpeed)
                        {
                            localPlayer.movementSpeed = 15f;
                        }

                        else
                        {
                            localPlayer.movementSpeed = 4.6f;
                        }
                    }
                }

                if (GUILayout.Button(MakeToggle("Climb", climbSpeed), buttonSize))
                {
                    climbSpeed = !climbSpeed;

                    var SOR = FindObjectOfType<StartOfRound>();

                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;

                        if (climbSpeed)
                        {
                            localPlayer.climbSpeed = 30f;
                        }

                        else
                        {
                            localPlayer.climbSpeed = 3f;
                        }
                    }
                }

                if (GUILayout.Button(MakeToggle("Jetpack", jetpack), buttonSize))
                {
                    jetpack = !jetpack;

                    var SOR = FindAnyObjectByType<StartOfRound>();
                    var localPlayer = SOR.localPlayerController;
                    if (SOR != null)
                    {
                        
                        if (jetpack)
                        {
                            localPlayer.jetpackControls = true;                          
                        }

                        else
                        {
                            localPlayer.jetpackControls = false;
                        }
                    }
                }
                GUILayout.EndVertical();
                //2 Столбец
                GUILayout.BeginVertical();
                if (GUILayout.Button("ShipDoorEnergy", buttonSize))
                {
                    hangarShipDoor = FindObjectOfType<HangarShipDoor>();

                    shipDoor = !shipDoor;

                    if (hangarShipDoor != null)
                    {

                        hangarShipDoor.SetDoorOpen();
                    }

                    else
                    {
                        hangarShipDoor.SetDoorClosed();
                    }
                }

                if(GUILayout.Button("Unlock all doors", buttonSize))
                {
                    foreach (DoorLock doorLock in FindObjectsOfType<DoorLock>())
                    {
                        doorLock.UnlockDoorSyncWithServer();
                    }
                }

                if(GUILayout.Button("Unlock big doors", buttonSize))
                {
                    terminalDoors = !terminalDoors;

                    foreach (TerminalAccessibleObject terminalAccessibleObject in FindObjectsOfType<TerminalAccessibleObject>())
                    {
                        terminalAccessibleObject.SetDoorOpenServerRpc(terminalDoors);
                    }
                }

                if(GUILayout.Button("Deactive mine", buttonSize))
                {
                    var landmines = FindObjectsOfType<Landmine>();
                    foreach (var landmine in landmines)
                    {
                        if (!landmine.hasExploded)
                        {
                            landmine.ExplodeMineServerRpc();
                        }
                    }
                }

                if(GUILayout.Button("Turret Stop", buttonSize))
                {
                    var turrets = FindObjectsOfType<Turret>();
                    foreach(var turret in turrets)
                    {
                        if (!turret.enabled)
                        {
                            turret.ToggleTurretClientRpc(false);
                        }
                    }
                }

                if (GUILayout.Button("Light", buttonSize))
                {
                    shipLights = FindObjectOfType<ShipLights>();
                    if (shipLights != null)
                    {
                        shipLight = !shipLight;
                        shipLights.ToggleShipLights();
                    }
                }

                if (GUILayout.Button("Force Start", buttonSize))
                {
                    startOf = FindObjectOfType<StartOfRound>();
                    startOf.StartGameServerRpc();
                }

                if (GUILayout.Button("End Game", buttonSize))
                {
                    startOf= FindObjectOfType<StartOfRound>();
                    startOf.EndGameServerRpc(0);
                }

                GUILayout.EndVertical();

                //3 Столбец
                GUILayout.BeginVertical();
                if (GUILayout.Button("Battery", buttonSize))
                {
                    Battery = !Battery;
                    var SOR = FindObjectOfType<StartOfRound>();
                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;
                       
                            foreach (GrabbableObject grabbable in localPlayer.ItemSlots)
                            {
                                if (grabbable != null)
                                {
                                    grabbable.insertedBattery.empty= false;
                                    grabbable.insertedBattery.charge = 100f;
                                }
                            }                       
                    }
                }

                if (GUILayout.Button("+3 ammo", buttonSize))
                {
                    var SOR = FindObjectOfType<StartOfRound>();
                    if (SOR != null)
                    {
                        var localPlayer = SOR.localPlayerController;

                        foreach (GrabbableObject grabbable in localPlayer.ItemSlots)
                        {
                            if (grabbable != null)
                            {
                                grabbable.GetComponent<ShotgunItem>().shellsLoaded = 999;
                            }
                        }
                    }
                }
                if (GUILayout.Button("Terminal ▶", buttonSize))
                {
                    menuMode = "TerminalInfo";
                }

                if (GUILayout.Button("Players ▶", buttonSize))
                {
                    menuMode = "Players";
                }

                if (GUILayout.Button("TP menu ▶", buttonSize))
                {
                    menuMode = "menuTP";
                }

                if (GUILayout.Button("Spawner ▶", buttonSize))
                {
                    menuMode = "spawnMenu";
                }

                if (GUILayout.Button("Flashlight ▶", buttonSize))
                {
                    menuMode = "flashlightMenu";
                }

                /* if (GUILayout.Button("Host menu", buttonSize))
                 {
                     TerminalHost = !TerminalHost;
                 } */

            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.DragWindow(dragMenu);
        }

        // 2 окно
        private void PlayerMenuB(int ID)
        {
            GUI.color = Color.cyan;

            PlayerControllerB[] players = FindObjectsOfType<PlayerControllerB>();

            if (players !=null && players.Length > 0)
            {

                foreach (PlayerControllerB player in players)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"Name: {player.playerUsername}");

                    GUILayout.BeginVertical();
                    GUILayout.Label($"Health: {player.health}");
                    GUILayout.Label($"Exhausted: {player.isExhausted}");
                    GUILayout.Label($"IsPlayerDead: {player.isPlayerDead}");
                    GUILayout.Label($"Speed: {player.movementSpeed}");
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical();

                    if (GUILayout.Button("Teleport", buttonSize))
                    {
                        Vector3 position = player.transform.position;
                        if (position != Vector3.zero)
                        {
                            selectedPlayer = player;
                            TeleportToSelectedPlayer();
                        }
                    }                 
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    GUI.DragWindow(dragPlayersMenu);
                }
            }
            if (GUILayout.Button("◀ Back", buttonSize))
            {
                menuMode = "Main";
            }

        }

        private void Flashlight(int ID)
        {
            GUI.color = Color.cyan;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (GUILayout.Button("Red", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.red;
            }
            if (GUILayout.Button("Blue", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.blue;
            }
            if (GUILayout.Button("Yellow", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.yellow;
            }
            if (GUILayout.Button("Green", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.green;
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            if (GUILayout.Button("Black", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.black;
            }
            if (GUILayout.Button("Cyan", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.cyan;
            }
            if (GUILayout.Button("Grey", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.grey;
            }
            if (GUILayout.Button("White", buttonSize))
            {
                fls = FindObjectOfType<FlashlightItem>();
                fls.flashlightBulb.color = Color.white;
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.Button("◀ Back", buttonSize))
            {
                menuMode = "Main";
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.DragWindow(dragFlashlight);
        }

        private void TeleportToSelectedPlayer()
        {
            if (selectedPlayer != null)
            {
                Vector3 targetPosition = selectedPlayer.transform.position;

                var SOR = FindObjectOfType<StartOfRound>();
                if (SOR != null)
                {
                    var localPlayer = SOR.localPlayerController;
                    if (localPlayer != null)
                    {
                        localPlayer.transform.position = targetPosition;
                    }
                }
            }
        }
      
        private void Spawner(int ID)
        {
            GUI.color = Color.cyan;       
            spawnerScroller = GUILayout.BeginScrollView(spawnerScroller);
            if (GUILayout.Button("◀ Back", buttonSize))
            {
                menuMode = "Main";
            }
            if (StartOfRound.Instance != null)
            {
                if (StartOfRound.Instance.allItemsList != null && StartOfRound.Instance.localPlayerController)
                {
                    foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
                    {
                        if (GUILayout.Button(item.itemName))
                        {                          
                            if (!NetworkManager.Singleton.IsConnectedClient || !NetworkManager.Singleton.IsServer)
                            {
                                return;
                            }
                            var localPlayer = StartOfRound.Instance.localPlayerController;
                            Vector3 PlayerPos = localPlayer.transform.position;
                            GameObject gameObject = Instantiate<GameObject>(item.spawnPrefab, PlayerPos, Quaternion.identity, StartOfRound.Instance.propsContainer);
                            gameObject.GetComponent<GrabbableObject>().fallTime = 0f;
                            gameObject.GetComponent<NetworkObject>().Spawn(false);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            GUI.DragWindow(dragSpawn);
        }
        private void menuTeleport(int ID)
        {
            GUI.color = Color.cyan;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //Первый ряд
            if (GUILayout.Button("Save Position", buttonSize))
            {
                SavePosition(1);
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("TP", buttonSize))
            {
                TeleportToSavedPosition(1);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Clear", buttonSize))
            {
                ClearSavePosition(1);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Второй ряд
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            
            if (GUILayout.Button("Save Position", buttonSize))
            {
                SavePosition(2);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("TP", buttonSize))
            {
                TeleportToSavedPosition(2);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Clear", buttonSize))
            {
                ClearSavePosition(2);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // Третий ряд
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (GUILayout.Button("Save Position", buttonSize))
            {
                SavePosition(3);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("TP", buttonSize))
            {
                TeleportToSavedPosition(3);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Clear", buttonSize))
            {
                ClearSavePosition(3);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Четвертый ряд
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (GUILayout.Button("Save Position", buttonSize))
            {
                SavePosition(4);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("TP", buttonSize))
            {
                TeleportToSavedPosition(4);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Clear", buttonSize))
            {
                ClearSavePosition(4);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Пятый ряд
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (GUILayout.Button("Save Position", buttonSize))
            {
                SavePosition(5);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("TP", buttonSize))
            {
                TeleportToSavedPosition(5);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Clear", buttonSize))
            {
                ClearSavePosition(5);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Шестой ряд
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            if (GUILayout.Button("Save Position", buttonSize))
            {
                SavePosition(6);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("TP", buttonSize))
            {
                TeleportToSavedPosition(6);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Clear", buttonSize))
            {
                ClearSavePosition(6);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (GUILayout.Button("◀ Back", buttonSize))
            {
                menuMode = "Main";
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow(dragMenuTP);
        }

        private void SavePosition(int ID)
        {
            var SOR = FindObjectOfType<StartOfRound>();
            if (SOR != null)
            {
                var localPlayer = SOR.localPlayerController;
                if (localPlayer != null)
                {
                    Vector3 playerPosition = localPlayer.transform.position;
                    PlayerPrefs.SetFloat("PosX_" + ID, playerPosition.x);
                    PlayerPrefs.SetFloat("PosY_" + ID, playerPosition.y);
                    PlayerPrefs.SetFloat("PosZ_" + ID, playerPosition.z);
                    PlayerPrefs.Save();
                }
            }
        }

        private void TeleportToSavedPosition(int ID)
        {
            var SOR = FindObjectOfType<StartOfRound>();
            if (SOR != null)
            {
                var localPlayer = SOR.localPlayerController;
                if (localPlayer != null)
                {

                    float posX = PlayerPrefs.GetFloat("PosX_" + ID);
                    float posY = PlayerPrefs.GetFloat("PosY_" + ID);
                    float posZ = PlayerPrefs.GetFloat("PosZ_" + ID);
                    Vector3 targetPosition = new Vector3(posX, posY, posZ);
                    localPlayer.transform.position = targetPosition;
                }
            }
        }

        private void ClearSavePosition(int ID)
        {
            var SOR = FindObjectOfType<StartOfRound>();
            if (SOR != null)
            {
                var localPlayer = SOR.localPlayerController;
                if (localPlayer != null)
                {
                    PlayerPrefs.DeleteKey("PosX_" + ID);
                    PlayerPrefs.DeleteKey("PosY_" + ID);
                    PlayerPrefs.DeleteKey("PosZ_" + ID);
                    PlayerPrefs.Save();
                }
            }
        }

        private void TeleportToShip()
        {
         

        }

        

        private void AddCredits()
        {
            terminal = FindObjectOfType<Terminal>();
            if (int.TryParse(creditInput, out int creditsToAdd))
            {
                terminal.groupCredits += creditsToAdd;            
            }
        }
       
        
        /* private void HostTerminal(int ID)
        {
            GUI.color = Color.red;
            terminal = FindObjectOfType<Terminal>();
            var SOR = FindObjectOfType<StartOfRound>();
            if (SOR != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();



                QuotaInput = GUILayout.TextField(QuotaInput, GUILayout.Width(100f));
                if (GUILayout.Button("Set Quota"))
                {
                    if (int.TryParse(QuotaInput, out int QuotaToAdd))
                    {
                        timeOf.quotaVariables.baseIncrease += QuotaToAdd;
                        timeOf.SetNewProfitQuota();
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            GUI.DragWindow(dragHost);
        } */

        private void Terminal(int ID)
        {
            GUI.color = Color.yellow;
            terminal = FindObjectOfType<Terminal>();
            if (terminal != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label($"Total credits: {terminal.groupCredits}");
                GUILayout.Label($"Total scrap value: {RoundManager.Instance.totalScrapValueInLevel}");
                GUILayout.Label($"Value of found scrap items:   {RoundManager.Instance.valueOfFoundScrapItems}");
                GUILayout.Label($"Alive players:   {StartOfRound.Instance.livingPlayers}");
                GUILayout.Label($"Total time:   {TimeOfDay.Instance.totalTime}");
                GUILayout.Label($"Total Quota:  {TimeOfDay.Instance.profitQuota}");
                GUILayout.Label($"Quota fulfilled:  {TimeOfDay.Instance.quotaFulfilled}");

                if (GUILayout.Button("◀ Back", buttonSize))
                {
                    menuMode = "Main";
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUI.DragWindow(dragTerminalInfo);
            }
        }
      
        private string MakeToggle(string name, bool toggle)
        {
            string status = toggle ? "<color=green>ON</color>" : "<color=red>OFF</color>";
            return $"{name} {status}";
        }

        private void Start()
        {
            Debug.Log("Текст");
        }
       
        private void NoclipMov()
        {
            var SOR = FindAnyObjectByType<StartOfRound>();
            if (SOR != null)
            {
                var localPlayer = SOR.localPlayerController;
                float speed = 25f;
                if (Keyboard.current.leftShiftKey.isPressed)
                {
                    speed *= 3f;
                }
                Vector2 vector = Keyboard.current.wKey.isPressed ? Vector2.up : (Keyboard.current.sKey.isPressed ? Vector2.down : (Keyboard.current.aKey.isPressed ? Vector2.left : (Keyboard.current.dKey.isPressed ? Vector2.right : Vector2.zero)));
                float y = 0f;
                if (Keyboard.current.spaceKey.isPressed)
                {
                    y = 1f;
                }
                else if (Keyboard.current.leftCtrlKey.isPressed)
                {
                    y = -1f;
                }
                Vector3 translation = new Vector3(vector.x, y, vector.y) * speed * Time.deltaTime;
                localPlayer.transform.Translate(translation);
            }
        }

        private bool noclip;
        private void Noclip()
        {
            var SOR = FindAnyObjectByType<StartOfRound>();
            if (SOR != null)
            {
                var localPlayer = SOR.localPlayerController;
                characterController = localPlayer.GetComponent<CharacterController>();
                Rigidbody rigidbody = localPlayer.GetComponent<Rigidbody>();
                if (characterControllers == 0f)
                {
                    characterControllers = characterController.radius;
                }
                characterController.enabled = !noclip;
                characterController.radius = float.PositiveInfinity;
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
                NoclipMov();
            }

        }

        private void NoClipTR()
        {
            characterController.enabled = true;
            characterController.radius = characterControllers;
        } 



        void Update()
        {
             Keyboard keyboard = Keyboard.current;
        
            if (keyboard != null && keyboard.backquoteKey.wasPressedThisFrame)
            {
                menuState = !menuState;
            }

            if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
               noclip = !noclip;
                if (!noclip)
                {
                    NoClipTR();
                }
            }

            if(noclip) 
            {
                Noclip();
            }
        }
    }
}
