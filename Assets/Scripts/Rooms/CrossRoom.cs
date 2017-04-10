using UnityEngine;
using System.Collections;

public class CrossRoom {

    public TileType[][] tiles;

    private int columns;
    private int rows;
    private int rect1X = 0;
    private int rect1Y = 0;
    private int rect2X = 0;
    private int rect2Y = 0;
    private int intersectionX = 0;
    private int intersectionY = 0;

    public void SetupRoom(int roomColumns, int roomRows, int minLenght, int maxLength)
    {
        columns = roomColumns;
        rows = roomRows;
        IntRandom roomLenght = new IntRandom(minLenght, maxLength);
        int[] lenghts = new int[4];

        InitializeTilesArray();

        for (int i = 0; i < 4; i ++)
        {
            lenghts[i] = roomLenght.Random;
        }

        while (lenghts[1] == lenghts[0])
            lenghts[1] = roomLenght.Random;

        while (lenghts[2] == lenghts[1] || lenghts[2] == lenghts[0])
            lenghts[2] = roomLenght.Random;

        while (lenghts[3] == lenghts[2] || lenghts[3] == lenghts[1] || lenghts[3] == lenghts[0])
            lenghts[3] = roomLenght.Random;

        System.Array.Sort(lenghts);

        CalculateLenghts(lenghts);

        FindIntersection();

        GenerateRoom();

    }

    void InitializeTilesArray()
    {
        tiles = new TileType[columns][];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = new TileType[rows];
    }

    void CalculateLenghts(int[] lenghts)
    {
        IntRandom binary = new IntRandom(0, 1);
        int biggest;

        biggest = binary.Random;
        rect1Y = lenghts[2 + biggest];
        rect2X = lenghts[2 + (biggest + 1) % 2];
        biggest = binary.Random;
        rect1X = lenghts[biggest];
        rect2Y = lenghts[(biggest + 1) % 2];
    }

    void FindIntersection()
    {
        intersectionX = Random.Range(0, rect2X - rect1X);
        intersectionY = Random.Range(0, rect1Y - rect2Y);
    }

    //Vou tomar o centro do tabuleiro como ponto de intersecao
    void GenerateRoom()
    {
        //Rect1
        for (int i = 0; i < rect1X; i++)
            for (int j = -intersectionY; j < rect1Y - intersectionY; j++)
                tiles[(columns / 2) + i][(rows / 2) + j] = TileType.Floor;

        //Rect1
        for (int i = -intersectionX; i < rect2X - intersectionX; i++)
            for (int j = 0; j < rect2Y; j++)
                tiles[(columns / 2) + i][(rows / 2) + j] = TileType.Floor;
    }
}
