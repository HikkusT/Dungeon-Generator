using UnityEngine;
using System.Collections;

namespace Library
{
    public class MyLibrary
    {
        int peso = 10;
        int xLenght = 0;
        int yLenght = 0;
        int finalX = 0;
        int finalY = 0;
        Node[][] seen;
        Node[][] visited;

        public int PathFiding (int startX, int startY, int endX, int endY, TileType[][] tiles)
        {
            Node current = null;
            int contador = 0;

            //Transformando em globais :)
            finalX = endX;
            finalY = endY;

            //Inicializar os arrays
            xLenght = tiles.GetLength(0);
            yLenght = tiles[0].Length;
            InitializeSets(tiles);

            //Adicionar o tile inicial 
            AddToSeen(startX, startY, -peso, null);

            //Loop do A*
            while(true)
            {
                current = FindNextNode();                      //Pegar o no não visitado com o menor f
                RemoveFromSeen(current);                       //Retirar da Lista Aberta
                AddToVisited(current);                         //Adicionar a lista Fechada


                //Se o atual for o node final, parar de buscar
                if (current.nodeX == endX && current.nodeY == endY || current.seen == false)
                    break;

                //Verificar cada vizinho. Se não puder ser ocupado, não fazer nada. Se puder e não foi visto, adicionar aos vistos. Se já foi visto, alterar o parente dependendo do g
                CheckNeighbours(current, tiles);
            }

            //Dar backtrack até chegar no inicio
            contador = 0;
            for (current = visited[endX][endY]; current.parent != null; current = current.parent)
                contador++;

            return contador;
        }

        private void InitializeSets(TileType[][] tiles)
        {
            seen = new Node[xLenght][];
            for (int i = 0; i < seen.Length; i++)
                seen[i] = new Node[yLenght];

            visited = new Node[xLenght][];
            for (int i = 0; i < visited.Length; i++)
                visited[i] = new Node[yLenght];

            for (int i = 0; i < xLenght; i++)
                for (int j = 0; j < yLenght; j++)
                {
                    seen[i][j] = new Node();
                    seen[i][j].seen = false;
                }

            for (int i = 0; i < xLenght; i++)
                for (int j = 0; j < yLenght; j++)
                {
                    visited[i][j] = new Node();
                    visited[i][j].parent = null;
                }
        }

        private void AddToSeen (int x, int y, int gValue, Node parent)
        {
            seen[x][y].nodeX = x;
            seen[x][y].nodeY = y;
            seen[x][y].gValue = gValue + peso;
            seen[x][y].hValue = Mathf.Abs(finalX - x) + Mathf.Abs(finalY - y);
            seen[x][y].fValue = seen[x][y].gValue + seen[x][y].hValue;
            seen[x][y].parent = parent;
            seen[x][y].seen = true;
        }

        private void AddToVisited (Node node)
        {
            visited[node.nodeX][node.nodeY] = node;
        }

        private Node FindNextNode ()
        {
            Node resposta = new Node();
            resposta.fValue = 10000;

            for (int i = 0; i < xLenght; i++)
                for (int j = 0; j < yLenght; j++)
                {
                    if (seen[i][j].seen && seen[i][j].fValue < resposta.fValue)
                        resposta = seen[i][j];
                }

            return resposta;
        }

        private void RemoveFromSeen (Node node)
        {
            seen[node.nodeX][node.nodeY] = new Node ();
        }

        private void CheckNeighbours (Node node, TileType[][] tiles)
        {
            int[] dx = new int[] { -1, 0, 1, 0 };
            int[] dy = new int[] { 0, 1, 0, -1 };

            for (int i = 0; i < 4; i++)
            {
                if (tiles[node.nodeX + dx[i]][node.nodeY + dy[i]] != TileType.Wall && visited[node.nodeX + dx[i]][node.nodeY + dy[i]].seen == false)
                {
                    if (seen[node.nodeX + dx[i]][node.nodeY + dy[i]].seen == true && seen[node.nodeX + dx[i]][node.nodeY + dy[i]].gValue > (node.gValue + peso))
                    {
                        seen[node.nodeX + dx[i]][node.nodeY + dy[i]].gValue = node.gValue + peso;
                        seen[node.nodeX + dx[i]][node.nodeY + dy[i]].parent = node;
                    }
                    else if (seen[node.nodeX + dx[i]][node.nodeY + dy[i]].seen == false)
                        AddToSeen(node.nodeX + dx[i], node.nodeY + dy[i], node.gValue, node);
                }
            }
        }
    }
}