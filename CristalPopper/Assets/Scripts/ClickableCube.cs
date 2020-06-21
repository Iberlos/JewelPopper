using UnityEngine;
using System.Collections;
using System.Linq;

public class ClickableCube : MonoBehaviour
{
    public bool rooted;
    public Vector2Int coord;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void Root()
    {
        
        if(!rooted)
        {
            rooted = true;
            m_cubeGrid.RootAt(coord, Vector2Int.up);
            m_cubeGrid.RootAt(coord, -Vector2Int.up);
            m_cubeGrid.RootAt(coord, Vector2Int.right);
            m_cubeGrid.RootAt(coord, -Vector2Int.right);
        }
    }

    public bool Activated
    {
        
        get
        {
            return m_Activated;
        }

        set
        {
            m_Activated = value;

            //Update the visuals since the activated value changes
            UpdateVisuals();
        }
    }

    public string colorKey
    {
        get
        {
            return m_colorKey;
        }

        set
        {
            m_colorKey = value;

            //Update the visuals since the color changed
            UpdateVisuals();
        }
    }

    public void ToggleActivated()
    {
        m_Activated = !m_Activated;

        //Update the visuals since the activated value changes
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (GetComponent<Renderer>().material == null)
        {
            return;
        }

        //Set the material color 
        if (m_Activated)
        {
            gameObject.SetActive(true);
            GetComponent<Renderer>().material.color = m_cubeGrid.GetSphereColorByKey(m_colorKey);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ActivateCube(string a_colorKey)
    {
        Activated = true;
        colorKey = a_colorKey;
    }

    public void SetGrid(CubeGrid a_cubeGrid)
    {
        m_cubeGrid = a_cubeGrid;
    }

    private bool m_Activated;
    private string m_colorKey;
    private CubeGrid m_cubeGrid;
}
