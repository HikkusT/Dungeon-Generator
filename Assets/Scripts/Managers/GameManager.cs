using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;
    private LevelGenerator levelGenerator;

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
            
    }

}
