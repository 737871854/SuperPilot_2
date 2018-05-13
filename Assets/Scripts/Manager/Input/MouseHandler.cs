using UnityEngine;
public class MouseHandler
{
    public bool TurnLeft()
    {
        return Input.GetKey(KeyCode.A);
    }
    public bool TurnRight()
    {
        return Input.GetKey(KeyCode.D);
    }
    public bool PullUp()
    {
        return Input.GetKey(KeyCode.W);
    }
    public bool PullDown()
    {
        return Input.GetKey(KeyCode.S);
    }
    public bool EnterSure()
    {
        return Input.GetKeyDown(KeyCode.KeypadEnter);
    }
    public bool Coin()
    {
        return Input.GetKeyDown(KeyCode.F1);
    }
    public bool Select()
    {
        return Input.GetKeyDown(KeyCode.PageDown);
    }
    public bool Confirm()
    {
        return Input.GetKeyDown(KeyCode.PageUp);
    }
    public bool Fire()
    {
        bool flag = false;
        if (Input.GetKeyDown(KeyCode.Space))
            flag = true;

        if (ioo.gameMode.State == GameState.Play && Input.GetKey(KeyCode.Space))
            flag = true;

        return flag;
    }

    public bool Gather()
    {
        return Input.GetKeyDown(KeyCode.DownArrow);
    }
}