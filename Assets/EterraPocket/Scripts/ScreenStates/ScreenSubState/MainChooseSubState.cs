using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.ScreenStates
{
  internal class MainChooseSubState : GameBaseState
  {
    public MainScreenState PlayScreenState => ParentState as MainScreenState;

    private readonly System.Random _random = new System.Random();

    private Button _btnPlay;

    private Button _btnReset;

    public MainChooseSubState(GameController flowController, GameBaseState parent)
        : base(flowController, parent) { }

    public override void EnterState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] EnterState");

      var floatBody = FlowController.VelContainer.Q<VisualElement>("FloatBody");
      floatBody.Clear();

      TemplateContainer scrollViewElement = ElementInstance("DemoGame/UI/Elements/ScrollViewElement");
      floatBody.Add(scrollViewElement);

      var scrollView = scrollViewElement.Q<ScrollView>("ScvElement");

      TemplateContainer elementInstance = ElementInstance("DemoGame/UI/Frames/ChooseFrame");

      _btnPlay = elementInstance.Q<Button>("BtnPlay");
      _btnPlay.SetEnabled(false);
      _btnPlay.RegisterCallback<ClickEvent>(OnBtnPlayClicked);

      var btnExit = elementInstance.Q<Button>("BtnExit");
      btnExit.RegisterCallback<ClickEvent>(OnBtnExitClicked);

      _btnReset = elementInstance.Q<Button>("BtnReset");
      _btnReset.SetEnabled(false);
      _btnReset.RegisterCallback<ClickEvent>(OnBtnResetClicked);


      // add element
      scrollView.Add(elementInstance);
    }

    public override void ExitState()
    {
      Debug.Log($"[{this.GetType().Name}][SUB] ExitState");
    }


    private void OnBtnPlayClicked(ClickEvent evt)
    {
      Debug.Log($"[{this.GetType().Name}][SUB] OnBtnPlayClicked");

    }

    private async void OnBtnResetClicked(ClickEvent evt)
    {
      Debug.Log($"[{this.GetType().Name}][SUB] OnBtnResetClicked");

    }

    private void OnBtnExitClicked(ClickEvent evt)
    {
      Debug.Log($"[{this.GetType().Name}][SUB] OnBtnExitClicked");
    }
  }
}