using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AvatarRobotBoneMap))]
public class PuppetComponent : MonoBehaviour
{
    private HumanBoneMap humanBoneMap;
    private AvatarRobotBoneMap avatarRobotBoneMap;
    private bool isBodyTracking;

    private void Awake()
    {
        avatarRobotBoneMap = GetComponent<AvatarRobotBoneMap>();
        humanBoneMap = new HumanBoneMap
        {
            BoneMaps = avatarRobotBoneMap.GetBoneMaps()
        };

        isBodyTracking = false;
    }

    public void StartTracking()
    {
        gameObject.SetActive(true);
        isBodyTracking = true;
    }

    public void StopTracking()
    {
        gameObject.SetActive(false);
        isBodyTracking = false;
    }
    
    private void LateUpdate()
    {
        if (!isBodyTracking) return;
        
        transform.localPosition = humanBoneMap.robotLocalPosition;
        var scale = humanBoneMap.robotLocalScale;
        scale.Set(-scale.x, scale.y, -scale.z);
        transform.localScale = scale;

        for (var i = 0; i < humanBoneMap.BoneMaps.Count; i++)
        {
            var jointIndex = (JointIndices)i;

            if (!humanBoneMap.BoneMaps.ContainsKey(jointIndex)) continue;

            var boneMap = humanBoneMap.BoneMaps[jointIndex];
            if (boneMap.avatarBone == null || boneMap.robotBone == null) continue;

            //root bone rotation shouldn't ever change, should start at hips
            if (jointIndex == JointIndices.Root) continue;

            boneMap.UpdateAvatarBoneLocalRotation();
        }
    }

    public void InitRobotPose(Transform robot, Vector3 initialPosition, Quaternion initialRotation,
        Dictionary<JointIndices, Transform> robotBoneMapping)
    {
        humanBoneMap.robotLocalPosition = initialPosition;
        humanBoneMap.robotLocalRotation = initialRotation;

        for (var i = 0; i < robotBoneMapping.Count; i++)
        {
            var jointIndex = (JointIndices)i;

            if (!humanBoneMap.BoneMaps.ContainsKey(jointIndex)) continue;

            var robotBone = robotBoneMapping[jointIndex];
            var boneMap = humanBoneMap.BoneMaps[jointIndex];
            boneMap.robotBone = robotBone;
        }
    }

    public void UpdateRobotPose(Vector3 localPosition, Quaternion localRotation, float estimatedHeight)
    {
        humanBoneMap.robotLocalPosition = localPosition;
        humanBoneMap.robotLocalRotation = localRotation;

        if (humanBoneMap.robotEstimatedHeight != estimatedHeight)
        {
            humanBoneMap.robotEstimatedHeight = estimatedHeight;
            var scale = humanBoneMap.robotLocalScale;
            scale.Set(-scale.x, scale.y, -scale.z);
            transform.localScale = scale;

            Debug.Log("Estimated Height Changed: " + estimatedHeight);
        }
    }

    public void DebugVector3ToString(string name, Vector3 vector3)
    {
        if (vector3 == Vector3.zero) return;

        var debugString = $"{name} | x:{vector3.x:0.##} y:{vector3.y:0.##} z:{vector3.z:0.##}";
        Debug.Log(debugString);
    }
}