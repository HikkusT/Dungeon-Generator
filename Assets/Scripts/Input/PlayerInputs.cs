using UnityEngine;
using System.Collections;

public class PlayerInputs
{

    public delegate void Execute(Moving actor);
    public Execute myExecute;

    public void MoveRight(Moving actor)
    {
        actor.Move(1, 0);
    }

    public void MoveUp(Moving actor)
    {
        actor.Move(0, 1);
    }

    public void MoveLeft(Moving actor)
    {
        actor.Move(-1, 0);
    }

    public void MoveDown(Moving actor)
    {
        actor.Move(0, -1);
    }
}
