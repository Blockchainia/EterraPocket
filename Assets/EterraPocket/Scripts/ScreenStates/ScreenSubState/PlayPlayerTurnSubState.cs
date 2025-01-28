using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace Assets.Scripts.ScreenStates
{
  internal class PlayPlayerTurnSubState : GameBaseState
  {
    private Button _btnStatusActionButton;
    private VisualElement _timeSpent;
    private VisualElement _timeLeft;
    private Coroutine _timerCoroutine;
    private float _elapsedPercentage;
    private VisualElement _opponentArrow; // Reference to the OpponentArrow UI element
    private VisualElement _playerArrow; // Reference to the OpponentArrow UI element


    public PlayPlayerTurnSubState(GameController flowController, GameBaseState parent)
        : base(flowController, parent) { }

    public override void EnterState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] EnterState");

      var root = FlowController.VelContainer.Q<VisualElement>("Screen");
      _btnStatusActionButton = root.Q<Button>("btnStatusActionButton");
      _timeSpent = root.Q<VisualElement>("TimeSpent");
      _timeLeft = root.Q<VisualElement>("TimeLeft");
      _opponentArrow = root.Q<VisualElement>("OpponentArrow"); // Get the OpponentArrow element
      _playerArrow = root.Q<VisualElement>("PlayerArrow"); // Get the OpponentArrow element

      // Disable the OpponentArrow
      if (_opponentArrow != null)
      {
        _opponentArrow.style.display = DisplayStyle.None;
        Debug.Log("OpponentArrow has been disabled.");
      }
      else
      {
        Debug.LogError("OpponentArrow element not found!");
      }
      // Enable the PlayerArrow
      if (_playerArrow != null)
      {
        _playerArrow.style.display = DisplayStyle.Flex;
        Debug.Log("PlayerArrow has been enabled.");
      }
      else
      {
        Debug.LogError("PlayerArrow element not found!");
      }

      // Reset and initialize timer
      ResetTimer();

      // Enable the button and set up its click handler
      _btnStatusActionButton.SetEnabled(true);
      _btnStatusActionButton.clicked += OnStatusActionClicked;

      // Ensure only one timer coroutine runs
      if (_timerCoroutine != null)
      {
        FlowController.StopCoroutine(_timerCoroutine);
        Debug.Log("Previous timer coroutine stopped in EnterState.");
      }

      _timerCoroutine = FlowController.StartCoroutine(StartTimer());
    }

    private void ResetTimer()
    {
      _elapsedPercentage = 0f; // Reset elapsed percentage
      _timeSpent.style.height = new StyleLength(Length.Percent(0));
      _timeLeft.style.height = new StyleLength(Length.Percent(100));
    }

    private IEnumerator StartTimer()
    {
      Debug.Log("Timer started in PlayPlayerTurnSubState.");
      while (_elapsedPercentage < 100f)
      {
        _elapsedPercentage += 3f;
        UpdateTime(_elapsedPercentage);
        yield return new WaitForSeconds(1f);
      }

      // Transition to the opponent's turn when the timer completes
      OnTimerComplete();
    }

    private void UpdateTime(float percentageElapsed)
    {
      Debug.Log($"[Timer] Updating time: {percentageElapsed}% elapsed.");

      // Ensure the style values are updated properly
      _timeSpent.style.height = new StyleLength(new Length(percentageElapsed, LengthUnit.Percent));
      _timeLeft.style.height = new StyleLength(new Length(100 - percentageElapsed, LengthUnit.Percent));
    }

    private void OnTimerComplete()
    {
      Debug.Log("Timer completed in PlayPlayerTurnSubState. Transitioning to PlayOpponentTurnSubState.");
      FlowController.ChangeScreenSubState(GameScreen.PlayScreen, GameSubScreen.PlayOpponentTurn);
    }

    private void OnStatusActionClicked()
    {
      Debug.Log("StatusActionButton clicked. Transitioning to PlayOpponentTurnSubState.");
      FlowController.ChangeScreenSubState(GameScreen.PlayScreen, GameSubScreen.PlayOpponentTurn);
    }

    public override void ExitState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] ExitState");

      // Stop and nullify the timer coroutine
      if (_timerCoroutine != null)
      {
        FlowController.StopCoroutine(_timerCoroutine);
        _timerCoroutine = null;
        Debug.Log("Timer coroutine stopped in ExitState.");
      }

      // Remove the click handler
      _btnStatusActionButton.clicked -= OnStatusActionClicked;
    }
  }
}