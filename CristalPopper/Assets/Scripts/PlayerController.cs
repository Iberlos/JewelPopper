using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    static public PlayerController m_instance;
    public Turret m_turret;
    float m_moveInput;
    float m_rotateInput;
    bool m_fireInput;

    void Start()
    {
        if(m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if(Application.platform != RuntimePlatform.Android)
        {
            m_moveInput = Input.GetAxis("Horizontal");
            m_rotateInput = Input.GetAxis("Rotate") != 0 ? Input.GetAxis("Rotate") : Input.GetAxis("Mouse X");
            if (!EventSystem.current.IsPointerOverGameObject())
                m_fireInput = Input.GetMouseButtonDown(0) || Input.GetKeyDown("space");
        }

        m_turret.Move(m_moveInput);
        m_turret.Rotate(m_rotateInput);
        m_turret.Aim();
        if(m_fireInput)
        {
            m_turret.Fire();
            m_fireInput = false;
        }
    }

    public void SetMoveInput(float a_moveInput)
    {
        m_moveInput = a_moveInput;
    }

    public void SetRotateInput(float a_rotateInput)
    {
        m_rotateInput = a_rotateInput;
    }

    public void SetFireInput(bool a_fireInput)
    {
        m_fireInput = a_fireInput;
    }
}
