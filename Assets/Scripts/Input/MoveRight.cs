using UnityEngine;
using System.Collections;

public class MoveRight : ICommand<Player> {

	public void Execute(Player actor)
    {
        actor.Move(1, 0);
    }
}
