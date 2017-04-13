using UnityEngine;
using System.Collections;

public class MoveSouth : ICommand<Player>
{

    public void Execute(Player actor)
    {
        actor.Move(0, -1);
    }
}