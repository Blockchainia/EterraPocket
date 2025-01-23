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

    public PlayInitSubState(GameController flowController, GameBaseState parent)
        : base(flowController, parent) { }

    public override void EnterState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] EnterState");

      var root = FlowController.VelContainer.Q<VisualElement>("Screen");

      _statusLabel = root.Q<Label>("StatusLabel");
      _statusActionButton = root.Q<Button>("StatusActionButton");
      _playerArrow = root.Q<VisualElement>("PlayerArrow");
      _opponentArrow = root.Q<VisualElement>("OpponentArrow");
      _timeSpent = root.Q<VisualElement>("TimeSpent");
      _timeLeft = root.Q<VisualElement>("TimeLeft");
      _playerScore = root.Q<Label>("lblPlayerScore");
      _opponentScore = root.Q<Label>("lblOpponentScore");

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
    }

    private void OnReadyClicked()
    {
      _statusActionButton.SetEnabled(false);
      FlowController.StartCoroutine(StartTimer());
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
    }

    public void UpdateTime(float percentageElapsed)
    {
      _timeSpent.style.height = new StyleLength(Length.Percent(percentageElapsed));
      _timeLeft.style.height = new StyleLength(Length.Percent(100 - percentageElapsed));
    }

    public void SetStatusText(string text)
    {
      _statusLabel.text = text;
    }

    public void SetArrowState(VisualElement arrow, bool enabled, Color backgroundColor)
    {
      arrow.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
      arrow.style.backgroundColor = new StyleColor(backgroundColor);
    }

    public override void ExitState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] ExitState");
    }
  }
}
