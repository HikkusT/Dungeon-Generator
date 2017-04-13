using UnityEngine;
using System.Collections;

public interface ICommand<T>
{
    void Execute(T actor);
}