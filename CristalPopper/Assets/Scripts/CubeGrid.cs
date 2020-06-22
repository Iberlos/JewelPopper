using UnityEngine;
using System.Collections.Generic;

public class CubeGrid : MonoBehaviour
{
    public int GridDimX = 15;
    public int GridDimY = 10;
    public float GridSpacing = 1.0f;
    public GameObject CubePrefab;
    public GameObject turretPrefab;
    public GameObject bouncerRailPrefab;
    public GameObject rootRailPrefab;
    public Camera gameCamera;
    public bool CalculatedCluster;


    private Turret turret;
    private List<RootRail> roots = default;
    private GameObject[] bouncers;
    private const int GX = 15, GY = 10;
    private List<ClickableCube> cubes = default;

    void Start()
    {
        PrepareForNextLevel();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PrepareForNextLevel();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
        }
    }

    private void PrepareForNextLevel()
    {
        //increse the level
        int level = GameManager.instance.Level;
        GridDimX += level;
        GridDimY += level;

        if (m_Grid != null)
        {
            //delete or recycle all the clickable cubes
            m_Grid = null;
        }
        if(m_VisitedCells != null)
        {
            m_VisitedCells = null;
        }
        if(m_CubeClusters != null)
        {
            m_CubeClusters.Clear();
        }

        //setup camera
        gameCamera.orthographicSize = (GridDimY + 7f)/2f;
        gameCamera.transform.position = new Vector3(GridDimX * 0.5f, gameCamera.orthographicSize - 2f, -10f);

        //instantiate the turret and give it its min and max pos an the cube grid
        if (turret == null)
        {
            turret = Instantiate(turretPrefab).GetComponent<Turret>();
            GameManager.instance.Turret = turret;
        }
        turret.transform.position = new Vector3(0.0f, GridDimY + 2.5f, 0.0f);
        turret.cubeGrid = this;
        turret.SetMoveLimits(new Vector3(2.0f, turret.transform.position.y, 0.0f), new Vector3(GridDimX - 3.0f, turret.transform.position.y, 0.0f));

        //Create a 2D array to hold the cubes, then generate the cubes in it
        if (m_Grid == null)
            m_Grid = new ClickableCube[GridDimX, GridDimY];

        //Create a grid of visited cells
        if(m_VisitedCells == null)
            m_VisitedCells = new bool[GridDimX, GridDimY];

        GenerateCubes();
        GenerateRoots();
        GenerateBouncers();

        if(m_CubeClusters == null)
            m_CubeClusters = new List<CubeCluster>();
        RecalcuateClusters();
        CalculatedCluster = false;

        GameManager.instance.Level++;
    }

    public void RecalcuateClusters()
    {
        //Clear all clusters
        m_CubeClusters.Clear();
        //Clear all visited cells
        ClearVisitedCells();
        //Create a container so we can store all cell that needs to be visited
        List<IntVector2> coordToVisit = new List<IntVector2>();
        IntVector2 startCoords;
        //Use function that can find non-visited cells
        //The logic below must be executed until all cells are visited
        while (FindNonVisitedCoord(out startCoords))
        {
            //Create cell cluster
            CubeCluster cubeCluster = new CubeCluster();
            cubeCluster.cubeGrid = this;
            //Add it to the existing list of clusters
            m_CubeClusters.Add(cubeCluster);
            //Add the same cell to the container which stores a coordinates/cells that needs to be visited
            coordToVisit.Add(startCoords);
            //The logic below must be executed until we have no more coordinates to be visited
            while(coordToVisit.Count > 0)
            {
                //Remove current cell from the list
                int indexToRemove = coordToVisit.Count - 1;
                IntVector2 currentCoords = coordToVisit[indexToRemove];
                coordToVisit.RemoveAt(indexToRemove);
                //Skip iteration if current coordinate/cell is already visited
                if(m_VisitedCells[currentCoords.x, currentCoords.y] == true)
                {
                    continue;
                }
                //Retrieve ClicableCube object based on current coordinates
                ClickableCube clickableCube = m_Grid[currentCoords.x, currentCoords.y];
                //Skip iteration if current ClicableCube is not active
                if(!clickableCube.Activated)
                {
                    //Set status of the current cell to visited
                    m_VisitedCells[currentCoords.x, currentCoords.y] = true;
                    continue;
                }
                //Add current ClicableCube to the cluster
                if(cubeCluster.AddCube(clickableCube, false))
                {
                    //Set status of the current cell to visited
                    m_VisitedCells[currentCoords.x, currentCoords.y] = true;
                    //Search around the current cell for enabled neighbour cells. If found add a coordinate to the need to visit container.
                    AddCoordsIfNeeded(currentCoords, new IntVector2(1, 0), ref coordToVisit);
                    AddCoordsIfNeeded(currentCoords, new IntVector2(-1, 0), ref coordToVisit);
                    AddCoordsIfNeeded(currentCoords, new IntVector2(0, 1), ref coordToVisit);
                    AddCoordsIfNeeded(currentCoords, new IntVector2(0, -1), ref coordToVisit);
                }
            }
        }       
    }

    //A helper function to add new coordinates to check in our search.
    //It will first create the new coords, then double check if the coordinates are valid before adding 
    //them to the list
    void AddCoordsIfNeeded(IntVector2 coords, IntVector2 checkDir, ref List<IntVector2> coordsToVisit)
    {
        IntVector2 nextCoords = coords + checkDir;

        if (AreCoordsValid(nextCoords))
        {
            coordsToVisit.Add(nextCoords);
        }
    }

    //This is a helper function to check if a set of coordinates are valid.  (out of bounds)
    bool AreCoordsValid(IntVector2 coords)
    {
        return coords.x >= 0 && coords.y >= 0 &&
            coords.x < m_Grid.GetLength(0) && coords.y < m_Grid.GetLength(1);
    }

    //Sets all of the visited cells back to non-visited
    void ClearVisitedCells()
    {
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                m_VisitedCells[x, y] = false;
            }
        }
    }

    //Finds a non-visited coordinate where we can start a search
    //it will return true if a non-vistited coordinate was found where we can start searching
    //
    //Note: This starts looking at the start of the grid every time.  One potential optimization
    //      could be to keep track of where you stopped looking last time, and then pick up from
    //      this location next time the function is executed.
    bool FindNonVisitedCoord(out IntVector2 nonVisitedCoord)
    {
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                if (m_Grid[x, y].Activated && !m_VisitedCells[x, y])
                {
                    nonVisitedCoord = new IntVector2(x, y);

                    return true;
                }
            }
        }

        //No non-visited activated coords found.  Set the value to an invalid coordinate
        //and return false
        nonVisitedCoord = new IntVector2(-1, -1);

        return false;
    }

    //Creates the cubes in the right position and puts them in the grid    
    private void GenerateCubes()
    {
        int cubeListIndex = 0;
        if(cubes == null)
        {
            cubes = new List<ClickableCube>();
        }
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY; ++y)
            {
                Vector3 offset = new Vector3(x * GridSpacing, y * GridSpacing, 0.0f);

                GameObject cubeObj;
                if (cubeListIndex >= cubes.Count)
                {
                    cubeObj = (GameObject)GameObject.Instantiate(CubePrefab);
                    cubes.Add(cubeObj.GetComponent<ClickableCube>());
                }
                else
                {
                    cubeObj = cubes[cubeListIndex].gameObject;
                }

                cubeObj.transform.position = offset + transform.position;

                cubeObj.transform.parent = transform;

                m_Grid[x, y] = cubeObj.GetComponent<ClickableCube>();
                m_Grid[x, y].SetGrid(this);
                m_Grid[x, y].coord = new Vector2Int(x, y);
                m_Grid[x, y].Activated = false;

                DebugUtils.Assert(m_Grid[x, y] != null, "Could not find clickableCube component.");
                cubeListIndex++;
            }
        }
        string[] keys = new string[4];
        keys[0] = "Red";
        keys[1] = "Green";
        keys[2] = "Blue";
        keys[3] = "Magenta";
        //Choose randomly if the cube should start activated or not
        for (int x = 0; x < GridDimX; ++x)
        {
            for (int y = 0; y < GridDimY-5; ++y)
            {
                m_Grid[x, y].ActivateCube(keys[Random.Range(0, keys.Length)]);
            }
        }
    }

    private void GenerateRoots()
    {
        //spawn as many roots as there are columns and set their root index to be the column index
        if(roots == null)
        {
            roots = new List<RootRail>();
            for (int x = 0; x < GridDimX; ++x)
            {
                Vector3 offset = new Vector3(x * GridSpacing, -1f * GridSpacing, 0.0f);
                RootRail root = Instantiate(rootRailPrefab).GetComponent<RootRail>();
                //position them on top of each column
                root.transform.position = offset;
                //Set the coord they link to
                root.RootCoord = new IntVector2(x, 0);
                roots.Add(root);
            }
        }
        else
        {
            for(int x = roots.Count; x < GridDimX; x++)
            {
                Vector3 offset = new Vector3(x * GridSpacing, -1f * GridSpacing, 0.0f);
                RootRail root = Instantiate(rootRailPrefab).GetComponent<RootRail>();
                //position them on top of each column
                root.transform.position = offset;
                //Set the coord they link to
                root.RootCoord = new IntVector2(x, 0);
                roots.Add(root);
            }
        }
    }

    private void GenerateBouncers()
    {
        if(bouncers == null)
        {
            //spawn two bouncers
            bouncers = new GameObject[2];
            bouncers[0] = Instantiate(bouncerRailPrefab);
            bouncers[1] = Instantiate(bouncerRailPrefab);
        }
        //position them on the side of the grids and scale them to be the size of all the rows
        bouncers[0].transform.position = new Vector3(-GridSpacing, GridDimY * GridSpacing * 0.5f, 0f);
        bouncers[0].transform.localScale = new Vector3(1f, (GridDimY + 6) * GridSpacing, 1f);
        bouncers[1].transform.position = new Vector3(GridDimX * GridSpacing, GridDimY * GridSpacing * 0.5f, 0f);
        bouncers[1].transform.localScale = new Vector3(1f, (GridDimY + 6) * GridSpacing, 1f);
    }

    public void ActivateCubeAt(string a_colorKey, Vector2Int a_coord)
    {
        if (AreCoordsValid(new IntVector2(a_coord.x, a_coord.y)))
        {
            m_Grid[a_coord.x, a_coord.y].ActivateCube(a_colorKey);
            NotifyClustersAboutCubeAdded(a_colorKey, a_coord);
        }
        else
        {
            if(a_coord.y >= GridDimY)
            {
                Debug.Log("YOU LOSE!");
            }
        }
    }

    public void NotifyClustersAboutCubeAdded(string a_colorKey, Vector2Int a_coord)
    {
        List<CubeCluster> clustersAdded = new List<CubeCluster>();
        foreach(CubeCluster cluster in m_CubeClusters)
        {
            if(cluster.ShouldAddCube(m_Grid[a_coord.x, a_coord.y], true))
            {
                clustersAdded.Add(cluster);
            }
        }
        if(clustersAdded.Count == 0)
        {
            m_CubeClusters.Add(new CubeCluster());
            m_CubeClusters[m_CubeClusters.Count - 1].cubeGrid = this;
            m_CubeClusters[m_CubeClusters.Count - 1].AddCube(m_Grid[a_coord.x, a_coord.y],true);
        }
        if(clustersAdded.Count >1)
        {
            MergeClusters(clustersAdded);
        }
        CheckRooted();
        RemoveBrokenClusters();
        if (m_CubeClusters.Count == 0)
        {
            Debug.Log("You win!");
            PrepareForNextLevel();
        }
    }

    void MergeClusters(List<CubeCluster> a_clusters)
    {
        List<ClickableCube> cubes = new List<ClickableCube>();
        foreach(CubeCluster cluster in a_clusters)
        {
            foreach(ClickableCube cube in cluster.m_Cubes)
            {
                bool found = false;
                foreach(ClickableCube savedCube in cubes)
                {
                    if(cube == savedCube)
                    {
                        found = true;
                        break;
                    }
                }
                if(!found)
                {
                    cubes.Add(cube);
                }
            }
            m_CubeClusters.Remove(cluster);
        }
        m_CubeClusters.Add(new CubeCluster());
        m_CubeClusters[m_CubeClusters.Count - 1].cubeGrid = this;
        foreach (ClickableCube savedCube in cubes)
        {
            m_CubeClusters[m_CubeClusters.Count - 1].AddCube(savedCube, true);
        }
    }

    public void RootAt(Vector2Int a_coord, Vector2Int a_direction)
    {
        IntVector2 rootCoord = new IntVector2(a_coord.x+a_direction.x, a_coord.y+a_direction.y);

        if(AreCoordsValid(rootCoord) && m_Grid[rootCoord.x, rootCoord.y].Activated)
        {
            m_Grid[rootCoord.x, rootCoord.y].Root();
        }
    }

    void CheckRooted()
    {
        foreach(ClickableCube cube in m_Grid)
        {
            cube.rooted = false;
        }
        for(int i = 0; i<GridDimX; i++)
        {
            if(m_Grid[i, 0].Activated)
            {
                m_Grid[i, 0].Root();
            }
        }
        foreach (CubeCluster cluster in m_CubeClusters)
        {
            cluster.CheckRooted();
        }
    }

    void RemoveBrokenClusters()
    {
        List<CubeCluster> brokenClusters = new List<CubeCluster>();
        foreach (CubeCluster cluster in m_CubeClusters)
        {
            if (cluster.broken) brokenClusters.Add(cluster);
        }
        foreach (CubeCluster cluster in brokenClusters)
        {
            int colorKey = -1;
            if(cluster.m_clusterColorKey == "Red")
            {
                colorKey = 0;
            }
            if (cluster.m_clusterColorKey == "Green")
            {
                colorKey = 1;
            }
            if (cluster.m_clusterColorKey == "Blue")
            {
                colorKey = 2;
            }
            if (cluster.m_clusterColorKey == "Magenta")
            {
                colorKey = 3;
            }

            GameManager.instance.AddJewels(colorKey, cluster.m_Cubes.Count + (brokenClusters.Count - 1)); //for every extra cluster broken add one jewel to each broken cluster coller as a reward
        }
        foreach (CubeCluster cluster in brokenClusters)
        { 
            m_CubeClusters.Remove(cluster);
        }
    }

    public Color GetSphereColorByKey(string a_key)
    {
        switch (a_key)
        {
            case "Red":
                {
                    return Color.red;
                }
            case "Green":
                {
                    return Color.green;
                }
            case "Blue":
                {
                    return Color.blue;
                }
            case "Magenta":
                {
                    return Color.magenta;
                }
            default:
                {
                    return Color.black;
                }
        }
    }

    private void OnDrawGizmos()
    {
        if(m_CubeClusters!= null)
        {
            foreach (CubeCluster cluster in m_CubeClusters) cluster.OnDrawGizmos();
        }
    }

    public Vector3 CoordToPosition(IntVector2 a_coord)
    {
        if(AreCoordsValid(a_coord))
        {
            return new Vector3(a_coord.x * GridSpacing, a_coord.y * GridSpacing, 0.0f);
        }
        else
        {
            return new Vector3(-100, -100, -100);
        }
    }

    //Using a 2D array to represent the grid
    ClickableCube[,] m_Grid;
    bool[,] m_VisitedCells;

    List<CubeCluster> m_CubeClusters;
}
