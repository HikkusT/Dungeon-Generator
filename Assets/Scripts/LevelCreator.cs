using UnityEngine;
using System.Collections;

public class LevelCreator : MonoBehaviour {

    public enum TileType
    {
        Wall, Floor,
    }

    public int columns = 200;
    public int rows = 200;
    public IntRandom numRooms = new IntRandom(8, 12);
    public IntRandom roomWidth = new IntRandom(5, 9);
    public IntRandom roomHeight = new IntRandom(5, 9);
    public IntRandom corridorLength = new IntRandom(4, 10);
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;

    private TileType[][] tiles;
}
