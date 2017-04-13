using UnityEngine;
using System.Collections;

public class InputHandler: MonoBehaviour{

    private KeyCode left = KeyCode.A;
    private KeyCode up = KeyCode.W;
    private KeyCode right = KeyCode.D;
    private KeyCode down = KeyCode.S;

    private MoveRight right_ = new MoveRight();
    private MoveLeft left_ = new MoveLeft();
    private MoveNorth up_ = new MoveNorth();
    private MoveSouth down_ = new MoveSouth();

    public ICommand<Player> HandleInput ()
    {
        if (Input.GetKeyDown(right))
            return right_;
        if (Input.GetKeyDown(left))
            return left_;
        if (Input.GetKeyDown(up))
            return up_;
        if (Input.GetKeyDown(down))
            return down_;

        return null;
    }
}
