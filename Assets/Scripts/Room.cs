using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room {

    public int highestX = 0;
    public int highestY = 0;
    public int lowestX = 10000;
    public int lowestY = 10000;
    public int eastDoor = 0;
    public int westDoor = 0;
    public int northDoor = 0;
    public int southDoor = 0;
    public TileType[][] tiles;

    private int columns;
    private int rows;
    private int smoothness;
    private float wallQuant;
    private int minLenght;
    private int maxLenght;

    public void SetupRoom (int roomColumns, int roomRows, int roomSmoothness, float roomWallQuant, int roomMinLenght, int roomMaxLenght, RoomType roomType)
    {
        columns = roomColumns;
        rows = roomRows;
        smoothness = roomSmoothness;
        wallQuant = roomWallQuant;
        minLenght = roomMinLenght;
        maxLenght = roomMaxLenght;

        InitializeTilesArray();

        switch(roomType)
        {
            case RoomType.Cave:
                Debug.Log("Fazendo Sala tipo Caverna");
                CaveRoom caveRoom = new CaveRoom ();
                caveRoom.SetupRoom(columns, rows, smoothness, wallQuant);
                tiles = caveRoom.tiles;
                break;
            case RoomType.Cross:
                Debug.Log("Fazendo Sala tipo Cross");
                CrossRoom crossRoom = new CrossRoom();
                crossRoom.SetupRoom(columns, rows, minLenght, maxLenght);
                tiles = crossRoom.tiles;
                break;
            default:
                Debug.Log("Caso Default: não deu certo");
                break;
        }

        GetHighestXandY();
        GetLowestXandY();
        Debug.Log("Sala criada. Tipo: " + roomType.ToString());
        Debug.Log("Dimensões: " + lowestX.ToString() + " " + lowestY.ToString() + " " + highestX.ToString() + " " + highestY.ToString());

        AddDoors();
    }

    int[][] InitializeArray(int[][] array)
    {
        array = new int[columns][];
        for (int i = 0; i < array.Length; i++)
            array[i] = new int[rows];
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                array[i][j] = 0;
        return array;
    }

    void InitializeTilesArray()
    {
        tiles = new TileType[columns][];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = new TileType[rows];
    }

    //Pegar o X e o Y máximos pra dar merge sem problemas
    void GetHighestXandY ()
    {
        highestX = 0;
        highestY = 0;
        for (int i = 0; i < columns; i ++)
            for (int j = 0; j < rows; j ++)
            {
                if (tiles[i][j] == TileType.Floor)
                {
                    if (i >= highestX)
                        highestX = i;
                    if (j >= highestY)
                        highestY = j;
                }
            }
    }

    //Pegar o X e o Y máximos pra dar portas e centralizar a primeira sala
    void GetLowestXandY()
    {
        lowestX = 10000;
        lowestY = 10000;
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
            {
                if (tiles[i][j] == TileType.Floor)
                {
                    if (i <= lowestX)
                        lowestX = i;
                    if (j <= lowestY)
                        lowestY = j;
                }
            }
    }

    //De cada lado da sala, vou adicionar uma possivel porta
    void AddDoors()
    {
        List<int> possibleIndex = new List<int>();

        //West Door
        possibleIndex.Clear();
        for (int j = 0; j < rows; j++)
            if (tiles[lowestX][j] == TileType.Floor)
                possibleIndex.Add(j);
        westDoor = possibleIndex[Random.Range(0, possibleIndex.Count)];

        //North Door
        possibleIndex.Clear();
        for (int i = 0; i < columns; i++)
            if (tiles[i][highestY] == TileType.Floor)
                possibleIndex.Add(i);
        northDoor = possibleIndex[Random.Range(0, possibleIndex.Count)];

        //East Door
        possibleIndex.Clear();
        for (int j = 0; j < rows; j++)
            if (tiles[highestX][j] == TileType.Floor)
                possibleIndex.Add(j);
        eastDoor = possibleIndex[Random.Range(0, possibleIndex.Count)];

        //West Door
        possibleIndex.Clear();
        for (int i = 0; i< columns; i++)
            if (tiles[i][lowestY] == TileType.Floor)
                possibleIndex.Add(i);
        southDoor = possibleIndex[Random.Range(0, possibleIndex.Count)];
    }

}
