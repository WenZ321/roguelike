using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerFollow : MonoBehaviour
{

    public GameObject Player;
    public Transform FollowTarget;
    private CinemachineVirtualCamera vcam;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.m_Lens.OrthographicSize = 5;
    }

    private void Awake()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }
    }

    private void Update()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
        }
        if (Player  != null)
        {
            FollowTarget = Player.transform;
            vcam.LookAt = FollowTarget;
            vcam.Follow = FollowTarget;
        }
    }
}