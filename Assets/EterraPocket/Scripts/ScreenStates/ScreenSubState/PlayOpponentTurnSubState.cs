using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace Assets.Scripts.ScreenStates
{
  internal class PlayOpponentTurnSubState : GameBaseState
  {
    private Button _btnStatusActionButton;
    private VisualElement _timeSpent;
    private VisualElement _timeLeft;
    private Coroutine _timerCoroutine;
    private float _elapsedPercentage;

    public PlayOpponentTurnSubState(GameController flowController, GameBaseState parent)
        : base(flowController, parent) { }

    public override void EnterState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] EnterState");

      var root = FlowController.VelContainer.Q<VisualElement>("Screen");
      _btnStatusActionButton = root.Q<Button>("btnStatusActionButton");
      _timeSpent = root.Q<VisualElement>("TimeSpent");
      _timeLeft = root.Q<VisualElement>("TimeLeft");

      // Reset and initialize the timer
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
      Debug.Log("Timer started in PlayOpponentTurnSubState.");
      while (_elapsedPercentage < 100f)
      {
        _elapsedPercentage += 3f;
        UpdateTime(_elapsedPercentage);
        yield return new WaitForSeconds(1f);
      }

      // Transition to the player's turn when the timer completes
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
      Debug.Log("Timer completed in PlayOpponentTurnSubState. Transitioning to PlayPlayerTurnSubState.");
      FlowController.ChangeScreenSubState(GameScreen.PlayScreen, GameSubScreen.PlayPlayerTurn);
    }

    private void OnStatusActionClicked()
    {
      Debug.Log("StatusActionButton clicked. Transitioning to PlayPlayerTurnSubState.");
      FlowController.ChangeScreenSubState(GameScreen.PlayScreen, GameSubScreen.PlayPlayerTurn);
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