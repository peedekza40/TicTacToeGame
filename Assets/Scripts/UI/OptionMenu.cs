using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    public GameObject NewGameButton;
    public GameObject ReplayControl;
    public Button BackwardButton;
    public Button ForwardButton;

    [Header("Events")]
    public GameEvent OnReplayControlAction;

    private void Awake() 
    {
        OnTogglePlay(true);
    }

    public void EnableReplayControl()
    {
        if(GameplayContext.instance.EnableReplayMode)
        {
            ReplayControl.SetActive(true);
        }
    }   

    public void EnableNewGameButton()
    {
        if(GameplayContext.instance.EnableReplayMode == false)
        {
            NewGameButton.SetActive(true);
        }
    } 

    public void OnTogglePlay(bool isOn)
    {
        if(isOn)
        {
            OnReplayControlAction.Raise(this, ReplayControlAction.Play);
            BackwardButton.interactable = false;
            ForwardButton.interactable = false;
        }
        else
        {
            OnReplayControlAction.Raise(this, ReplayControlAction.Pause);
            BackwardButton.interactable = true;
            ForwardButton.interactable = true;
        }
    }

    public void OnBackwardClick()
    {
        OnReplayControlAction.Raise(this, ReplayControlAction.Backward);
    }

    public void OnForwardClick()
    {
        OnReplayControlAction.Raise(this, ReplayControlAction.Forward);
    }
}
