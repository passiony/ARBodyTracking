using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AvatarParent : MonoBehaviour
{
    private int index;
    private PuppetComponent m_Current;
    private PuppetComponent[] Cloths;

    private Renderer controlledRobotRenderer;
    private Transform controlledRobot;

    private AvatarRobotTestSuite avatarRobotTestSuite;
    private ARHumanBodyManager arHumanBodyManager;

    void Start()
    {
        Cloths = gameObject.GetComponentsInChildren<PuppetComponent>(true);
        arHumanBodyManager = FindObjectOfType<ARHumanBodyManager>();
        avatarRobotTestSuite = FindObjectOfType<AvatarRobotTestSuite>();
        foreach (var cloth in Cloths)
        {
            cloth.StopTracking();
        }
    }

    public void ShowNext()
    {
        if (m_Current)
        {
            index++;
            if (index >= Cloths.Length)
            {
                index = 0;
            }
        }
        else
        {
            index = 0;
            m_Current = Cloths[index];
        }

        m_Current = Cloths[index];
        foreach (var cloth in Cloths)
        {
            cloth.StopTracking();
        }
        m_Current.StartTracking();
    }

    public void InitRobotPose(Transform robot, Vector3 initialPosition, Quaternion initialRotation,
        Dictionary<JointIndices, Transform> robotBoneMapping)
    {
        controlledRobot = robot;
        controlledRobotRenderer = controlledRobot.gameObject.GetComponentInChildren<Renderer>();

        foreach (var cloth in Cloths)
        {
            cloth.InitRobotPose(robot, initialPosition, initialRotation, robotBoneMapping);
        }

        ShowNext();
    }

    public void UpdateRobotPose(Vector3 localPosition, Quaternion localRotation, float estimatedHeight)
    {
        if (m_Current)
        {
            m_Current.UpdateRobotPose(localPosition, localRotation, estimatedHeight);
        }
    }

    public void HideRobot()
    {
        if (controlledRobotRenderer == null) return;

        controlledRobotRenderer.enabled = !controlledRobotRenderer.enabled;
    }

    public void StartBodyTracking()
    {
        if (controlledRobotRenderer != null)
        {
            controlledRobotRenderer.enabled = true;
        }

        avatarRobotTestSuite.StopTest();
        arHumanBodyManager.SetTrackablesActive(true);
        arHumanBodyManager.enabled = true;
        // HideRobot();
    }

    public void StartTest()
    {
        arHumanBodyManager.SetTrackablesActive(false);
        arHumanBodyManager.enabled = false;
        ShowNext();
        avatarRobotTestSuite.StartTest();
    }
}