using UnityEngine;

public class A_StarNode
{
    private bool isWall;        // 벽 확인용
    private Vector2Int position;
    private int g;              // 현재까지 비용
    private int h;              // 목표까지 비용
    private A_StarNode parent;  // 부모 노드및 탐색한 노드, 경로를 출력하기 위해 필요

    public bool IsWall { get { return isWall; } }
    public Vector2Int Position { get { return position; } }
    public int G { get { return g; } set { g = value; } }
    public int H { get { return h; } set { h = value; } }
    public int F { get { return G + H; } } // 도착까지의 총 비용 
    public A_StarNode Parent { get { return parent; } set { parent = value; } }



    public A_StarNode(Vector2Int position, bool isWall)
    {
        this.position = position;
        this.isWall = isWall;
        g = int.MaxValue;
    }

    public void ResetCost() // 초기화 용 매서드 
    {
        G = int.MaxValue;
        H = 0;
        Parent = null;
    }

}
