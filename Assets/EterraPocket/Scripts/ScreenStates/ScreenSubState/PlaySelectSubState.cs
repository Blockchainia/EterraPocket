using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.ScreenStates
{
  internal class PlaySelectSubState : GameBaseState
  {
    private readonly System.Random _random = new System.Random();

    private Button _btnPlay;

    private Button _btnReset;

    public PlaySelectSubState(GameController flowController, GameBaseState parent)
        : base(flowController, parent) { }

    public override void EnterState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] EnterState");
    }

    public override void ExitState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] ExitState");
    }
  }
}