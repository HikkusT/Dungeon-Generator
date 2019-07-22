using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour{

    public static GameManager instance = null;

    public float turnDelay = 0.2f;
    public bool enemiesTurn = false;
    public bool playersTurn = true;

    private LevelGenerator levelGenerator;
    private List<Observer> observers;

    void Awake ()
    {
        //Tornando-o um singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this.gameObject);

        //Garantindo que não vai ser destruido on load
        DontDestroyOnLoad(this.gameObject);

        levelGenerator = GetComponent<LevelGenerator>();

        observers = new List<Observer>();
        observers.Clear();    
    }

    void Update()
    {
        if (enemiesTurn || playersTurn)
            return;

        StartCoroutine(EnemiesTurn());
    }

    IEnumerator EnemiesTurn()
    {
        enemiesTurn = true;

        Notify();

        yield return new WaitForSeconds(turnDelay);

        enemiesTurn = false;
        playersTurn = true;
    }

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
