using UnityEngine;
using System.Collections;

public class Player : Moving {

    private InputHandler inputHandler;
    private ICommand<Player> command;

    protected override void Start()
    {
        inputHandler = GetComponent<InputHandler>();

        base.Start();
    }

    public override bool Move(int xDir, int yDir)
    {
        bool canMove = base.Move(xDir, yDir);

        if (canMove)
            GameManager.instance.playersTurn = false;

        return canMove;
    }

    void Update ()
    {
        if (!GameManager.instance.playersTurn)
            return;

        command = inputHandler.HandleInput();

        if (command != null)
            command.Execute(this);
    }
}
