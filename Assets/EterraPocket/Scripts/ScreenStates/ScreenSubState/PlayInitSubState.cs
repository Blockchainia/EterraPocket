using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace Assets.Scripts.ScreenStates
{
  internal class PlayInitSubState : GameBaseState
  {
    private Label _statusLabel;
    private Button _statusActionButton;
    private VisualElement _playerArrow;
    private VisualElement _opponentArrow;
    private VisualElement _timeSpent;
    private VisualElement _timeLeft;
    private Label _playerScore;
    private Label _opponentScore;
    private Button _btnPlayerReady;
    private Label _lblOpponentReady;

    private Coroutine _timerCoroutine;

    public PlayInitSubState(GameController flowController, GameBaseState parent)
        : base(flowController, parent) { }

    public override void EnterState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] EnterState");

      var root = FlowController.VelContainer.Q<VisualElement>("Screen");

      _statusLabel = root.Q<Label>("lblStatusLabel");
      _statusActionButton = root.Q<Button>("btnStatusActionButton");
      _playerArrow = root.Q<VisualElement>("PlayerArrow");
      _opponentArrow = root.Q<VisualElement>("OpponentArrow");
      _timeSpent = root.Q<VisualElement>("TimeSpent");
      _timeLeft = root.Q<VisualElement>("TimeLeft");
      _playerScore = root.Q<Label>("lblPlayerScore");
      _opponentScore = root.Q<Label>("lblOpponentScore");
      _btnPlayerReady = root.Q<Button>("btnPlayerReady");
      _lblOpponentReady = root.Q<Label>("lblOpponentReady");

      InitializeGameState();
    }

    private void InitializeGameState()
    {
      _playerScore.text = "5";
      _opponentScore.text = "5";
      _timeSpent.style.height = new StyleLength(Length.Percent(0));
      _timeLeft.style.height = new StyleLength(Length.Percent(100));

      _statusActionButton.text = "Ready";
      _statusActionButton.SetEnabled(true);
      _statusActionButton.clicked += OnReadyClicked;

      _btnPlayerReady.SetEnabled(true);
      _lblOpponentReady.style.display = DisplayStyle.Flex;
      _btnPlayerReady.clicked += OnPlayerReadyClicked;

      // Start the timer coroutine
      if (_timerCoroutine != null)
      {
        FlowController.StopCoroutine(_timerCoroutine);
      }
      _timerCoroutine = FlowController.StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
      float elapsedPercentage = 0f;
      while (elapsedPercentage < 100f)
      {
        elapsedPercentage += 3f;
        UpdateTime(elapsedPercentage);
        yield return new WaitForSeconds(1f);
      }

      Debug.Log("Timer finished in PlayInitSubState.");
    }

    public void UpdateTime(float percentageElapsed)
    {
      Debug.Log($"[Timer] Updating time: {percentageElapsed}% elapsed.");

      _timeSpent.style.height = new StyleLength(Length.Percent(percentageElapsed));
      _timeLeft.style.height = new StyleLength(Length.Percent(100 - percentageElapsed));
    }

    private void OnReadyClicked()
    {
      Debug.Log("Ready button clicked.");
      _statusActionButton.SetEnabled(false);
      // No need to start the timer here as it's already started in InitializeGameState
    }

    private void OnPlayerReadyClicked()
    {
      Debug.Log("Player ready button clicked. Transitioning to PlayPlayerTurnSubState.");
      _btnPlayerReady.style.display = DisplayStyle.None;
      _lblOpponentReady.style.display = DisplayStyle.None;
      _btnPlayerReady.SetEnabled(false);
      _lblOpponentReady.SetEnabled(false);

      FlowController.ChangeScreenSubState(GameScreen.PlayScreen, GameSubScreen.PlayPlayerTurn);
    }

    public override void ExitState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] ExitState");

      // Stop the timer coroutine if it is running
      if (_timerCoroutine != null)
      {
        FlowController.StopCoroutine(_timerCoroutine);
        _timerCoroutine = null;
        Debug.Log("Timer coroutine stopped in ExitState.");
      }

      // Unsubscribe from button events
      _statusActionButton.clicked -= OnReadyClicked;
      _btnPlayerReady.clicked -= OnPlayerReadyClicked;
    }
  }
}