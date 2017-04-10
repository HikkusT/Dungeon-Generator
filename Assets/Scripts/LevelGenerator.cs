using UnityEngine;
using System.Collections;
using Library;

public class LevelGenerator : MonoBehaviour {

    private MyLibrary Lib = new MyLibrary();

    public int columns = 200;
    public int rows = 200;
    public int smoothness = 5;
    public float wallQuant = 0.54f;
    public int minLenght = 4;
    public int maxLenght = 12;
    public int maxIterations = 10;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] doorTiles;

    private TileType[][] tiles;
    private TileType[][] secTiles;                               //"Segundo plano" de tiles que vai dar merge com o principal sempre que add uma nova sala
    private int[][] doors;
    private Room rooms;
    private GameObject boardHolder;

    private int[][] pathFiding;

    private void Start()
    {
        boardHolder = new GameObject("BoardHolder");

        InitializeTilesArray();

        GenerateFirstRoom();

        StartCoroutine("GenerateOtherRooms");

        //InstantiateTiles();
    }


    //Gerar a primeira sala centralizada
    void GenerateFirstRoom()
    {
        //Ver quantos tipos de sala tem
        int numRoomTypes = System.Enum.GetNames(typeof(RoomType)).Length;
        IntRandom intRoomType = new IntRandom(0, numRoomTypes);
        RoomType roomType;

        //Escolher uma aleatória
        roomType = (RoomType)intRoomType.Random;
        Debug.Log("Primeira sala. Tipo escolhido: " + roomType.ToString());

        //Gerando
        rooms = new Room ();
        rooms.SetupRoom(columns, rows, smoothness, wallQuant, minLenght, maxLenght, roomType);
        secTiles = rooms.tiles;

        //O deslocamento do grid secundário será de metade das (colunas/fileiras - lowestX/lowestY) para centralizar, menos um ajuste de coluna/20 para considerar a largura da sala
        int deltaX = (columns/ 2) - rooms.lowestX  - (columns / 10);
        int deltaY = (rows/ 2) - rooms.lowestY - (rows / 10);
        Merge(deltaX, deltaY);
    }

    void InitializeTilesArray()
    {
        tiles = new TileType[columns][];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = new TileType[rows];
    }
    
    //Função ficou bem mal escrita. Reescrever
    IEnumerator GenerateOtherRooms()
    {
        //Ver quantos tipos de sala tem
        int numRoomTypes = System.Enum.GetNames(typeof(RoomType)).Length;
        IntRandom intRoomType = new IntRandom(0, numRoomTypes);
        RoomType roomType;

        int contador = 0;
        int xMerge = 0;
        int yMerge = 0;
        bool foundSpace = false;

        while(contador < maxIterations)
        {
            foundSpace = false;

            roomType = (RoomType)intRoomType.Random;

            rooms = new Room();
            rooms.SetupRoom(columns, rows, smoothness, wallQuant, minLenght, maxLenght, roomType);
            secTiles = rooms.tiles;

            for (int i = 2; i < columns - (rooms.highestX - rooms.lowestX) - 2 && !foundSpace; i++)
            {
                for (int j = 2; j < rows - (rooms.highestY - rooms.lowestY) - 2 && !foundSpace; j++)
                {
                    if (CheckForSpace(i, j))
                    {
                        Debug.Log("FOUND");
                        foundSpace = true;
                        xMerge = i;
                        yMerge = j;
                    }
                    //yield return new WaitForEndOfFrame();
                    //yield return null;
                }
                yield return null;
            }

            if (foundSpace)
                Merge(xMerge - rooms.lowestX, yMerge - rooms.lowestY);
            contador++;
        }

        AddDoor();

        InstantiateTiles();
    }

    //Checando se eu posso adicionar uma sala numa posição
    bool CheckForSpace(int x, int y)
    {
        //Debug.Log("Checking: x = " + x.ToString() + " y = " + y.ToString());
        bool resposta = true;

        for (int i = 0; i < (rooms.highestX - rooms.lowestX + 1) && resposta; i++)
            for (int j = 0; j < (rooms.highestY - rooms.lowestY + 1) && resposta; j++)
                if ((tiles[x + i][y + j] == TileType.Floor) && (secTiles[rooms.lowestX + i][rooms.lowestY + j] == TileType.Floor)  || CheckTile(x, y, i, j))
                {
                    resposta = false;
                    //Debug.Log("Sobreposição: x = " + x.ToString() + " y = " + y.ToString());
                }

        if (resposta)
        {
            resposta = false;
            //Checando por portas

            //West Door
            if (tiles[x - 1][y + (rooms.westDoor - rooms.lowestY)] == TileType.Wall && tiles[x - 2][y + (rooms.westDoor - rooms.lowestY)] == TileType.Floor)
            {
                resposta = true;
                Debug.Log("Possivel porta Oeste: x = " + x.ToString() + " y = " + y.ToString());
            }

            //North Door
            else if (tiles[x + (rooms.northDoor - rooms.lowestX)][y + (rooms.highestY - rooms.lowestY) + 1] == TileType.Wall && tiles[x + (rooms.northDoor - rooms.lowestX)][y + (rooms.highestY - rooms.lowestY) + 2] == TileType.Floor)
            {
                resposta = true;
                Debug.Log("Possivel porta Norte: x = " + x.ToString() + " y = " + y.ToString());
            }

            //EastDoor
            else if (tiles[x + (rooms.highestX - rooms.lowestX) + 1][y + (rooms.eastDoor - rooms.lowestY)] == TileType.Wall && tiles[x + (rooms.highestX - rooms.lowestX) + 2][y + (rooms.eastDoor - rooms.lowestY)] == TileType.Floor)
            {
                resposta = true;
                Debug.Log("Possivel porta Leste: x = " + x.ToString() + " y = " + y.ToString());
            }
            //South Door
            else if (tiles[x + (rooms.southDoor - rooms.lowestX)][y - 1] == TileType.Wall && tiles[x + (rooms.southDoor - rooms.lowestX)][y - 2] == TileType.Floor)
            {
                resposta = true;
                Debug.Log("Possivel porta Sul: x = " + x.ToString() + " y = " + y.ToString());
            }
        }

        return resposta;
    }

    //Dando merge no grid principal
    void Merge(int deltaX, int deltaY)
    {
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                if (((i - deltaX) >= 0) && ((j- deltaY) >= 0) && ((i - deltaX) < columns) && ((j - deltaY) < rows))
                    if (secTiles[i - deltaX][j - deltaY] == TileType.Floor)
                        tiles[i][j] = TileType.Floor;
    }

    void AddDoor()
    {
        doors = InitializeArray(doors);

        for (int i = 2; i < columns - 2; i ++)
            for (int j = 2; j < rows - 2; j ++)
                if (tiles[i][j] == TileType.Floor)
                {
                    if (tiles[i + 1][j] == TileType.Wall && tiles[i + 2][j] == TileType.Floor)
                        if (Lib.PathFiding(i, j, i + 2, j, tiles) == 0 || Lib.PathFiding(i, j, i + 2, j, tiles) >= 20)
                            PunchDoor(i + 1, j);
                    if (tiles[i][j + 1] == TileType.Wall && tiles[i][j + 2] == TileType.Floor)
                        if (Lib.PathFiding(i, j, i, j + 2, tiles) == 0 || Lib.PathFiding(i, j, i, j + 2, tiles) >= 20)
                            PunchDoor(i, j + 1);
                    if (tiles[i - 1][j] == TileType.Wall && tiles[i - 2][j] == TileType.Floor)
                        if (Lib.PathFiding(i, j, i -2, j, tiles) == 0 || Lib.PathFiding(i, j, i - 2, j, tiles) >= 20)
                            PunchDoor(i - 1, j);
                    if (tiles[i][j - 1] == TileType.Wall && tiles[i][j - 2] == TileType.Floor)
                        if (Lib.PathFiding(i, j, i, j - 2, tiles) == 0 || Lib.PathFiding(i, j, i, j - 2, tiles) >= 20)
                            PunchDoor(i, j - 1);
                }
    }

    void FindPath(int startX, int startY, int endX, int endY, int peso)
    {
        pathFiding[startX][startY] = peso;
        if (tiles[startX + 1][startY] == TileType.Floor && pathFiding[startX + 1][startY] == 0)
            FindPath(startX + 1, startY, endX, endY, peso + 1);
        if (tiles[startX][startY + 1] == TileType.Floor && pathFiding[startX][startY + 1] == 0)
            FindPath(startX, startY + 1, endX, endY, peso + 1);
        if (tiles[startX - 1][startY] == TileType.Floor && pathFiding[startX - 1][startY] == 0)
            FindPath(startX - 1, startY, endX, endY, peso + 1);
        if (tiles[startX][startY - 1] == TileType.Floor && pathFiding[startX][startY - 1] == 0)
            FindPath(startX, startY - 1, endX, endY, peso + 1);
    }

    bool CheckForDoor(int x, int y)
    {
        bool resposta = false;
        if (doors[x][y] == 1)
            return true;
        else if (pathFiding[x][y] == 1)
            return false;
        else if (pathFiding[x][y] == 0)
            return false;

        if (pathFiding[x + 1][y] != 0 && pathFiding[x + 1][y] < pathFiding[x][y])
            resposta = CheckForDoor(x + 1, y);
        else if (pathFiding[x][y + 1] != 0 && pathFiding[x][y + 1] < pathFiding[x][y])
            resposta = CheckForDoor(x, y + 1);
        else if (pathFiding[x - 1][y] != 0 && pathFiding[x - 1][y] < pathFiding[x][y])
            resposta = CheckForDoor(x - 1, y);
        else if (pathFiding[x][y - 1] != 0 && pathFiding[x][y - 1] < pathFiding[x][y])
            resposta = CheckForDoor(x, y - 1);

        return resposta;
    }

    void PunchDoor(int x, int y)
    {
        doors[x][y] = 1;
        tiles[x][y] = TileType.Floor;
    }
    
    //Finalização
    void InstantiateTiles()
    {
        for (int i = 0; i < tiles.Length; i++)
            for (int j = 0; j < tiles[i].Length; j++)
            {
                //InstantiateFromArray(floorTiles, i, j);
                if (tiles[i][j] == TileType.Wall)
                    InstantiateFromArray(doorTiles, wallTiles, i, j);
                else
                    InstantiateFromArray(doorTiles, floorTiles, i, j);
            }
    }

    void InstantiateFromArray(GameObject[] doorPrefabs, GameObject[] prefabs, int xCoord, int yCoord)
    {
        //Pego um sprite randomico(vai ser util depois
        int randomIndex = Random.Range(0, prefabs.Length);
        int randomDoorIndex = Random.Range(0, doorPrefabs.Length);
        //Posicao que vai dar instantiate
        Vector3 position = new Vector3(xCoord, yCoord, 0f);
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;
        if (doors[xCoord][yCoord] == 1)
        {
            position = new Vector3(xCoord, yCoord, -1f);
            GameObject doorInstance = Instantiate(doorPrefabs[randomDoorIndex], position, Quaternion.identity) as GameObject;
            doorInstance.transform.parent = boardHolder.transform;
        }
        tileInstance.transform.parent = boardHolder.transform;
    }

    bool CheckTile(int x, int y, int i, int j)
    {
        int[] xOffsets = new int[] { -1, 0, 1, 1, 1, 0, -1, -1 };
        int[] yOffsets = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
        int cont = 0;

        for (int a = 0; a < 8; a++)
            if ((tiles[x + i + xOffsets[a]][y + j + yOffsets[a]] == TileType.Floor) && secTiles[rooms.lowestX + i][rooms.lowestY + j] == TileType.Floor)
                cont++;

        return (cont != 0);
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
}
