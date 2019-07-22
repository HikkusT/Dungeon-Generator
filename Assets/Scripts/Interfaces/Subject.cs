using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Subject
{
    private List<Observer> observers;

	public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        observers.Remove(observer);
    }

    protected void Notify()
    {
        foreach (Observer observer in observers)
        {
            observer.onNotify();
        }
    }
}
