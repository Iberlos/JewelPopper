using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Turret : MonoBehaviour
{
    public GameObject canon;
    public GameObject jewel;
    public GameObject gohstJewel;
    public Transform muzzle;
    public GameObject spherePrefab;
    public float rotateSpeed;
    public float moveSpeed;
    public float sphereSpeed;
    [HideInInspector]
    public CubeGrid cubeGrid;

    private string m_nextSphereColorKey;
    private float m_angle = 180;
    private float m_moveAlpha = 0.5f;
    private Vector3 m_minPos, m_maxPos;
    private LineRenderer m_lineRenderer;

    private void Start()
    {
        PlayerController.m_instance.m_turret = this;
        gohstJewel = Instantiate(gohstJewel);
        m_lineRenderer = GetComponent<LineRenderer>();
        Reload(GameManager.instance.GetJewel);
    }

    public void Move(float a_moveInput)
    {
        m_moveAlpha += -a_moveInput * (moveSpeed / Vector3.Distance(m_minPos, m_maxPos)) * Time.deltaTime;
        m_moveAlpha = Mathf.Clamp(m_moveAlpha, 0, 1);
        transform.position = Vector3.Lerp(m_minPos, m_maxPos, m_moveAlpha);
    }

    public void Rotate(float a_rotateInput)
    {
        m_angle += -a_rotateInput * rotateSpeed * Time.deltaTime;
        m_angle = Mathf.Clamp(m_angle, 120, 240);
        canon.transform.rotation = Quaternion.Euler(0.0f, 0.0f, m_angle);
    }

    public void Aim()
    {
        //Raycast from the muzzle forward untill the hit object is either the rootRail or a validCristal
        //place a phantom jewel on the hit slot to show where it would land.
        if (m_nextSphereColorKey != "")
        {
            bool coodFound = false;
            List<Vector3> positions = new List<Vector3>();
            positions.Add(muzzle.transform.position);
            Vector3 rayDir = muzzle.transform.forward;
            Vector3 rayStart = muzzle.transform.position;
            while (!coodFound)
            {
                RaycastHit outHit = new RaycastHit();
                if (Physics.SphereCast(rayStart, 0.1f, rayDir, out outHit, 100.0f, LayerMask.GetMask("ClickableCube")))
                {
                    Vector3 endPoint = outHit.point + outHit.normal * 0.5f;
                    positions.Add(endPoint);
                    if(outHit.collider.gameObject.tag == "Jewel")
                    {
                        Vector2Int destinationCoord = outHit.collider.gameObject.GetComponent<ClickableCube>().coord;
                        Vector2 fnormal = new Vector2(outHit.normal.x, outHit.normal.y);
                        if (fnormal.x != 0 && fnormal.y != 0)
                        {
                            if (Mathf.Abs(fnormal.x) >= Mathf.Abs(fnormal.y))
                            {
                                fnormal.y = 0;
                            }
                            else
                            {
                                fnormal.x = 0;
                            }
                        }
                        Vector2Int adjustment = new Vector2Int(Mathf.RoundToInt(fnormal.x), Mathf.RoundToInt(fnormal.y));
                        destinationCoord += adjustment;

                        gohstJewel.transform.position = cubeGrid.CoordToPosition(new IntVector2(destinationCoord.x, destinationCoord.y));
                        m_lineRenderer.SetPositions(positions.ToArray());
                        coodFound = true;
                    }
                    if(outHit.collider.gameObject.tag == "Bouncer")
                    {
                        Vector3 projection = Vector3.Project(rayDir, outHit.normal);
                        rayDir -= 2*projection;
                        rayStart = endPoint;
                    }
                    if (outHit.collider.gameObject.tag == "Root")
                    {
                        IntVector2 destinationCoord = outHit.collider.gameObject.GetComponent<RootRail>().RootCoord;
                        gohstJewel.transform.position = cubeGrid.CoordToPosition(new IntVector2(destinationCoord.x, destinationCoord.y));
                        m_lineRenderer.SetPositions(positions.ToArray());
                        coodFound = true;
                    }
                }
                else
                {
                    break;
                }
            }
            m_lineRenderer.positionCount = positions.Count;
        }
    }

    public void Fire()
    { 
        if (m_nextSphereColorKey != "")
        {
            string colorKey = m_nextSphereColorKey;

            bool coordFound = false;
            List<Vector3> positions = new List<Vector3>();
            positions.Add(muzzle.transform.position);
            Vector3 rayDir = muzzle.transform.forward;
            Vector3 rayStart = muzzle.transform.position;

            while (!coordFound)
            {
                RaycastHit outHit = new RaycastHit();
                if (Physics.SphereCast(rayStart, 0.1f, rayDir, out outHit, 100.0f, LayerMask.GetMask("ClickableCube")))
                {
                    Vector3 endPoint = outHit.point + outHit.normal * 0.5f;
                    positions.Add(endPoint);

                    if (outHit.collider.gameObject.tag == "Jewel")
                    {
                        Vector2Int destinationCoord = outHit.collider.gameObject.GetComponent<ClickableCube>().coord;
                        Vector2 fnormal = new Vector2(outHit.normal.x, outHit.normal.y);
                        if (fnormal.x != 0 && fnormal.y != 0)
                        {
                            if (Mathf.Abs(fnormal.x) >= Mathf.Abs(fnormal.y))
                            {
                                fnormal.y = 0;
                            }
                            else
                            {
                                fnormal.x = 0;
                            }
                        }
                        Vector2Int adjustment = new Vector2Int(Mathf.RoundToInt(fnormal.x), Mathf.RoundToInt(fnormal.y));
                        destinationCoord += adjustment;

                        Sphere sphere = Instantiate(spherePrefab, muzzle.transform.position, muzzle.transform.rotation, null).GetComponent<Sphere>();
                        //Call initialize on Sphere passing in the coord, the endPoint, the collor, the sphereSpeed, the cubeGrid reference and the turret reference
                        sphere.Initialize(positions.ToArray(), destinationCoord, colorKey, sphereSpeed, cubeGrid, this);
                        coordFound = true;
                    }
                    if (outHit.collider.gameObject.tag == "Bouncer")
                    {
                        Vector3 projection = Vector3.Project(rayDir, outHit.normal);
                        rayDir -= 2 * projection;
                        rayStart = endPoint;
                    }
                    if (outHit.collider.gameObject.tag == "Root")
                    {
                        IntVector2 temp = outHit.collider.gameObject.GetComponent<RootRail>().RootCoord;
                        Vector2Int destinationCoord = new Vector2Int(temp.x, temp.y);
                        Sphere sphere = Instantiate(spherePrefab, muzzle.transform.position, muzzle.transform.rotation, null).GetComponent<Sphere>();
                        //Call initialize on Sphere passing in the coord, the endPoint, the collor, the sphereSpeed, the cubeGrid reference and the turret reference
                        sphere.Initialize(positions.ToArray(), destinationCoord, colorKey, sphereSpeed, cubeGrid, this);
                        coordFound = true;
                    }
                }
                else
                {
                    break;
                }
            }
            if(coordFound)
            {
                gohstJewel.gameObject.SetActive(false);
                m_nextSphereColorKey = "";
                jewel.GetComponent<Renderer>().material.color = cubeGrid.GetSphereColorByKey(m_nextSphereColorKey);
            }
        }
    }

    public void Reload(int colorKey)
    {
        if (colorKey == -1)
            return;
        gohstJewel.gameObject.SetActive(true);
        string[] keys = new string[4];
        keys[0] = "Red";
        keys[1] = "Green";
        keys[2] = "Blue";
        keys[3] = "Magenta";
        m_nextSphereColorKey = keys[colorKey];
        jewel.GetComponent<Renderer>().material.color = cubeGrid.GetSphereColorByKey(m_nextSphereColorKey);
        Color gohstColor = cubeGrid.GetSphereColorByKey(m_nextSphereColorKey);
        gohstColor.a = 0.5f;
        gohstJewel.GetComponent<Renderer>().material.color = gohstColor;
        m_lineRenderer.startColor = m_lineRenderer.endColor = gohstColor;
    }

    public int Unload()
    {
        int colorIndex = 0;
        if (m_nextSphereColorKey == "Red")
            colorIndex = 0;
        if (m_nextSphereColorKey == "Green")
            colorIndex = 1;
        if (m_nextSphereColorKey == "Blue")
            colorIndex = 2;
        if (m_nextSphereColorKey == "Magenta")
            colorIndex = 3;
        m_nextSphereColorKey = "";
        jewel.GetComponent<Renderer>().material.color = cubeGrid.GetSphereColorByKey(m_nextSphereColorKey);
        return colorIndex;
    }

    public void SetMoveLimits(Vector3 a_minPos, Vector3 a_maxPos)
    {
        m_minPos = a_minPos;
        m_maxPos = a_maxPos;
    }
}
