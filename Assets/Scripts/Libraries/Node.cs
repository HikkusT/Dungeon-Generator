using UnityEngine;
using System.Collections;

public class Node {

    public int nodeX = 0;
    public int nodeY = 0;
    public int fValue = 0;
    public int gValue = 0;
    public int hValue = 0;
    public Node parent = null;
    public bool seen = false;

}

