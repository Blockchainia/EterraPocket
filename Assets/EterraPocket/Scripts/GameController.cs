using Assets.Scripts.ScreenStates;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
  public enum GameScreen
  {
    StartScreen,
    MainScreen,
    PlayScreen,
  }

  public enum GameSubScreen
  {
    MainChoose,
    Play,
    PlaySelect,
    PlayFinish,
    PlayWaiting,
  }

  public class GameController : ScreenStateMachine<GameScreen, GameSubScreen>
  {
    //   internal NetworkManager Network => NetworkManager.GetInstance();
    //   internal StorageManager Storage => StorageManager.GetInstance();

    internal readonly RandomNumberGenerator Random = RandomNumberGenerator.Create();

    public Vector2 ScrollOffset { get; set; }

    // public CacheData CacheData { get; private set; }

    public VisualElement VelContainer { get; private set; }

    private new void Awake()
    {
      base.Awake();
      //Your code goes here

      //CacheData = new CacheData();
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
      var root = GetComponent<UIDocument>().rootVisualElement;
      VelContainer = root.Q<VisualElement>("VelContainer");

      // VelContainer.RemoveAt(1);

      // if (VelContainer.childCount > 1)
      // {
      //   Debug.Log("Plaese remove development work, before starting!");
      //   return;
      // }

      // call insital flow state
      ChangeScreenState(GameScreen.StartScreen);
    }

    protected override void InitializeStates()
    {
      _stateDictionary.Add(GameScreen.StartScreen, new StartScreen(this));

      var mainScreen = new MainScreenState(this);
      _stateDictionary.Add(GameScreen.MainScreen, mainScreen);

      var mainScreenSubStates = new Dictionary<GameSubScreen, IScreenState>
            {
                { GameSubScreen.MainChoose, new MainChooseSubState(this, mainScreen) },
                { GameSubScreen.Play, new MainPlaySubState(this, mainScreen) },
            };
      _subStateDictionary.Add(GameScreen.MainScreen, mainScreenSubStates);

      var playScreen = new PlayScreenState(this);
      _stateDictionary.Add(GameScreen.PlayScreen, playScreen);

      var playScreenSubStates = new Dictionary<GameSubScreen, IScreenState>
            {
                { GameSubScreen.PlaySelect, new PlaySelectSubState(this, playScreen) },
                { GameSubScreen.PlayFinish, new PlayFinishSubState(this, playScreen) },
                { GameSubScreen.PlayWaiting, new PlayWaitingSubState(this, playScreen) },
            };
      _subStateDictionary.Add(GameScreen.PlayScreen, playScreenSubStates);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
      // Method intentionally left empty.
    }
  }
}
