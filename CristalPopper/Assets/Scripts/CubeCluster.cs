using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeCluster
{
    public CubeGrid cubeGrid;
    public bool broken;

    public CubeCluster()
    {

        m_Cubes = new List<ClickableCube>();

        if (GameObject.Find("CubeGrid").GetComponent<CubeGrid>())
        {
            //Choosing a random color for the cluster
            m_clusterColorKey = "";

            GameObject.Find("CubeGrid").GetComponent<CubeGrid>().CalculatedCluster = true;
        }
    }

    public bool AddCube(ClickableCube cube, bool removeClusters)
    {
        if(m_clusterColorKey == "")
        {
            m_clusterColorKey = cube.colorKey;
            m_minCoords = cube.coord;
            m_maxCoords = cube.coord;
        }

        if (cube.colorKey == m_clusterColorKey)
        {
            m_Cubes.Add(cube);
            m_minCoords.x = Math.Min(m_minCoords.x, cube.coord.x);
            m_minCoords.y = Math.Min(m_minCoords.y, cube.coord.y);
            m_maxCoords.x = Math.Max(m_maxCoords.x, cube.coord.x);
            m_maxCoords.y = Math.Max(m_maxCoords.y, cube.coord.y);
            if (removeClusters && m_Cubes.Count >= 3)
            {
                Break();
            }
            return true;
        }
        return false;

    }

    public bool ShouldAddCube(ClickableCube a_cube, bool removeClusters)
    {
        if(a_cube.colorKey == m_clusterColorKey)
        {
            if(a_cube.coord.x >= m_minCoords.x-1 && a_cube.coord.x <= m_maxCoords.x + 1)
            {
                if (a_cube.coord.y >= m_minCoords.y - 1 && a_cube.coord.y <= m_maxCoords.y + 1)
                {
                    foreach(ClickableCube cube in m_Cubes)
                    {
                        if(a_cube.coord + Vector2Int.up == cube.coord || a_cube.coord - Vector2Int.up == cube.coord || a_cube.coord + Vector2Int.right == cube.coord || a_cube.coord - Vector2Int.right == cube.coord)
                        {
                            return AddCube(a_cube, removeClusters);
                        }
                    }
                }
            }
        }
        return false;
    }

    public void OnDrawGizmos()
    {
        if(m_clusterColorKey != "")
        {
            Gizmos.color = cubeGrid.GetSphereColorByKey(m_clusterColorKey);
            foreach (ClickableCube cube in m_Cubes)
            {
                Vector3 pos = new Vector3(cube.coord.x, cube.coord.y, 1.0f) - cubeGrid.transform.position;
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
    }

    public void Break()
    {
        broken = true;
        foreach(ClickableCube cube in m_Cubes)
        {
            cube.Activated = false;
        }
    }

    public void CheckRooted()
    {
        if(!m_Cubes[0].rooted)
        {
            Break();
        }
    }

    public List<ClickableCube> m_Cubes { private set; get; }

    public string m_clusterColorKey { private set; get; }

    Vector2Int m_minCoords;
    Vector2Int m_maxCoords;
}
