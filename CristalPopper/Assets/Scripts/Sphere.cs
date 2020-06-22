using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sphere : MonoBehaviour
{
    CubeGrid m_cubeGrid;
    Turret m_turret;
    float m_sphereSpeed;
    string m_colorKey;
    Vector2Int m_destinationCoord;
    Vector3[] m_positions;
    Vector3 m_endPoint;
    Vector3 m_startPoint;
    int m_posIndex;

    void Update()
    {
        float totalDistance = Vector3.Distance(m_startPoint, m_endPoint);
        float remainingDistance = Vector3.Distance(transform.position, m_endPoint);
        float alpha = Mathf.Clamp((totalDistance-remainingDistance + m_sphereSpeed) / totalDistance, 0.0f, 1.0f);
        transform.position = Vector3.Lerp(m_startPoint, m_endPoint, alpha);
        transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y + 360 * Time.deltaTime, 0.0f);
        if(alpha == 1.0f)
        {
            m_posIndex++;
            if (m_posIndex < m_positions.Length)
            {
                m_startPoint = m_endPoint;
                m_endPoint = m_positions[m_posIndex];
            }
            else
            {
                m_cubeGrid.ActivateCubeAt(m_colorKey, m_destinationCoord);
                m_turret.Reload(GameManager.instance.GetJewel);
                Destroy(gameObject);
            }
        }
    }

    public void Initialize(Vector3[] a_endPoint, Vector2Int a_destinationCoord, string a_colorKey, float a_sphereSpeed, CubeGrid a_cubeGrid, Turret a_turret)
    {
        m_positions = a_endPoint;
        m_endPoint = m_positions[0];
        m_colorKey = a_colorKey;
        m_destinationCoord = a_destinationCoord;
        m_sphereSpeed = a_sphereSpeed;
        m_cubeGrid = a_cubeGrid;
        m_turret = a_turret;
        m_startPoint = transform.position;
        GetComponent<Renderer>().material.color = m_cubeGrid.GetSphereColorByKey(a_colorKey);
        m_posIndex = 0;
    }
}
