using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.ScreenStates
{
  public class PlayScreenState : GameBaseState
  {
    public int PlayerIndex { get; private set; }

    public PlayScreenState(GameController _flowController)
        : base(_flowController)
    {
      // load assets here
      //TileCardElement = Resources.Load<VisualTreeAsset>($"DemoGame/UI/Elements/TileCardElement");
    }

    public override void EnterState()
    {
      Debug.Log($"[{this.GetType().Name}] EnterState");

      // filler is to avoid camera in the ui
      var topFiller = FlowController.VelContainer.Q<VisualElement>("VelTopFiller");
      //topFiller.style.backgroundColor = GameConstant.ColorDark;

      var visualTreeAsset = Resources.Load<VisualTreeAsset>($"DemoGame/UI/Screens/PlayScreenUI");
      var instance = visualTreeAsset.Instantiate();
      instance.style.width = new Length(100, LengthUnit.Percent);
      instance.style.height = new Length(98, LengthUnit.Percent);

      // add container
      FlowController.VelContainer.Add(instance);

      // load initial sub state
      //FlowController.ChangeScreenSubState(GameScreen.PlayScreen, GameSubScreen.PlaySelect);

      // initial update
    }

    public override void ExitState()
    {
      Debug.Log($"[{this.GetType().Name}] ExitState");

      // remove container
      FlowController.VelContainer.RemoveAt(1);
    }



  }
}