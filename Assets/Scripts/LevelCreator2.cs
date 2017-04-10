using UnityEngine;
using System.Collections;

public class LevelCreator2 : MonoBehaviour {

    /*public enum TileType
    {
        Wall, Floor,
    }*/
    
    public int columns = 200;
    public int rows = 200;
    public int smoothness = 5;
    public float wallQuant = 0.42f;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;

    private TileType[][] tiles;
    private GameObject boardHolder;

    private void Start()
    {
        boardHolder = new GameObject("BoardHolder");

        InitializeTilesArray();

        GenerateCaves();

        GenerateFirstRoom();

        InstantiateTiles();
    }

    void InitializeTilesArray()
    {
        tiles = new TileType[columns][];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = new TileType[rows];
    }

    void GenerateCaves()
    {
        //Vou fazer 42% dos tiles serem aleatoriamente paredes. O resto vai ser chao
        int totalTiles = columns * rows;
        int wallTiles = Mathf.RoundToInt(wallQuant * totalTiles);
        //Variaveis para pegar randomicamente
        int xPos;
        int yPos;
        IntRandom xPosGen = new IntRandom(0, columns);
        IntRandom yPosGen = new IntRandom(0, rows);

        //Inicializando tudo como chao
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                tiles[i][j] = TileType.Floor;

        //Sorteando 2 numeros aleatorios, 1 pra linha e 1 pra coluna, wallTiles-vezes
        for (int i = 0; i < wallTiles; i++)
        {
            xPos = xPosGen.Random;
            yPos = yPosGen.Random;
            while (tiles[xPos][yPos] == TileType.Wall)
            {
                xPos = xPosGen.Random;
                yPos = yPosGen.Random;
            }
            tiles[xPos][yPos] = TileType.Wall;
        }

        //Fazendo as bordas serem paredes
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
            {
                if (i == 0 || i == columns - 1 || j == 0 || j == rows - 1)
                    tiles[i][j] = TileType.Wall;
            }

        //Removendo o ruido
        for (int i = 0; i < smoothness; i++)
            SmoothLevel();

    }

    void GenerateFirstRoom()
    {
        int xStart = 0;
        int yStart = 0;
        int blobLenght = 0;
        int biggestBlob = 0;
        int[][] visited = new int[1][];                 //Guardar quem ja foi visitado

        visited = InitializeArray(visited);             //Inicializando

        //Achar o maior blob
        for (int i = 1; i < columns - 1; i++)
            for (int j = 1; j < rows - 1; j++)
            {
                blobLenght = 0;
                if (visited[i][j] == 0)
                    blobLenght = VisitBlob(i, j, ref visited);
                if (blobLenght >= biggestBlob)
                {
                    biggestBlob = blobLenght;
                    xStart = i;
                    yStart = j;
                }
            }

        //Marcar como o maior blob
        MarkBiggestBlob(xStart, yStart, ref visited);

        //Apagar todos menos o maior blob
        GetBiggestBlob(visited);
    }

    void InstantiateTiles()
    {
        for (int i = 0; i < tiles.Length; i++)
            for (int j = 0; j < tiles[i].Length; j++)
            {
                //InstantiateFromArray(floorTiles, i, j);
                if (tiles[i][j] == TileType.Wall)
                    InstantiateFromArray(wallTiles, i, j);
                else
                    InstantiateFromArray(floorTiles, i, j);
            }
    }

    //Reduzindo o "ruido" da geracao randomica
    void SmoothLevel()
    {
        int[][] shouldChange = new int[1][];

        //Zerando a matriz
        /*shouldChange = new int[columns][];
        for (int i = 0; i < shouldChange.Length; i++)
            shouldChange[i] = new int[rows];
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                shouldChange[i][j] = 0;*/
        shouldChange = InitializeArray(shouldChange);

        //Checando por criterios de ruido
        for (int i = 1; i < columns - 1; i++)
            for (int j = 1; j < rows - 1; j++)
                if (CheckTile(i, j))
                    shouldChange[i][j] = 1;

        //Trocando
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                if(shouldChange[i][j] == 1)
                {
                    int intTile = (int)tiles[i][j];
                    intTile = (intTile + 1) % 2;
                    tiles[i][j] = (TileType)intTile;
                }
    }

    //Inicializar uma array de inteiros
    int[][] InitializeArray (int[][] array)
    {
        array = new int[columns][];
        for (int i = 0; i < array.Length; i++)
            array[i] = new int[rows];
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                array[i][j] = 0;
        return array;
    }

    //Verificando os tiles adjacentes para fazer o SmoothLevel
    bool CheckTile(int x, int y)
    {
        int[] xOffsets = new int[] { -1, 0, 1, 1, 1, 0, -1, -1 };
        int[] yOffsets = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
        int cont = 0;
        int threshold;

        for (int i = 0; i < 8; i++)
            if (tiles[x][y] == tiles[x + xOffsets[i]][y + yOffsets[i]])
                cont++;

        if (tiles[x][y] == TileType.Floor)
            threshold = 3;
        else
            threshold = 2;

        return (cont <= threshold);
    }

    //Dado um ponto, verificar o blob ao qual ele pertence
    int VisitBlob(int x, int y, ref int[][] visited)
    {
        int contador = 0;
        
        //Fazer uma Recursão
        VisitTile(x, y, ref contador, ref visited);

        return contador;
    }

    //Recursao de percuso de tiles
    void VisitTile(int x, int y, ref int cont, ref int[][] visited)
    {
        //Se ainda nao foi visitado e é chão, contar e marcar como visitado
        if(tiles[x][y] == TileType.Floor && visited[x][y] == 0)
        {
            cont++;
            visited[x][y] = 1;

            //Visitar cada um dos adjacentes
            VisitTile(x + 1, y, ref cont, ref visited);
            VisitTile(x - 1, y, ref cont, ref visited);
            VisitTile(x, y + 1, ref cont, ref visited);
            VisitTile(x, y - 1, ref cont, ref visited);
        }
        else
            visited[x][y] = 1;                                  //Marcar como visitado mesmo se for wall
    }

    //Parecido com o VisitTile
    void MarkBiggestBlob(int x, int y, ref int[][] visited)
    {
        //Transformar todos do blob em 2
        if (tiles[x][y] == TileType.Floor && visited[x][y] != 2)
        {
            visited[x][y] = 2;

            //Visitar cada um dos adjacentes
            MarkBiggestBlob(x + 1, y, ref visited);
            MarkBiggestBlob(x - 1, y, ref visited);
            MarkBiggestBlob(x, y + 1, ref visited);
            MarkBiggestBlob(x, y - 1, ref visited);
        }
    }

    //Tudo que nao e dois vira wall
    void GetBiggestBlob(int[][] visited)
    {
        for (int i = 0; i < columns; i++)
            for (int j = 0; j < rows; j++)
                if (visited[i][j] != 2)
                    tiles[i][j] = TileType.Wall;
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord)
    {
        //Pego um sprite randomico(vai ser util depois
        int randomIndex = Random.Range(0, prefabs.Length);
        //Posicao que vai dar instantiate
        Vector3 position = new Vector3(xCoord, yCoord, 0f);
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, Quaternion.identity) as GameObject;
        tileInstance.transform.parent = boardHolder.transform;
    }
}
