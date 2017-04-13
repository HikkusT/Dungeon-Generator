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

    void Update ()
    {
        command = inputHandler.HandleInput();

        command.Execute(this);
    }
}
