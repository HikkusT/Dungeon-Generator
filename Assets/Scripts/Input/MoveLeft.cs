using UnityEngine;
using System.Collections;

public class MoveLeft : ICommand<Player>
{

    public void Execute(Player actor)
    {
        actor.Move(-1, 0);
    }
}