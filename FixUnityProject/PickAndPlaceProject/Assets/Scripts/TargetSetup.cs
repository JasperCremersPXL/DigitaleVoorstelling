using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Robotics.PickAndPlace {
    public class TargetSetup : MonoBehaviour
{
    public GameObject target;
    public GameObject targetPlacement;
    public GameObject publisher;

    public void SetupTargetOnClick() {
        target.name = "Target";
        targetPlacement.GetComponent<TargetPlacementScript>().m_Target = target;
        publisher.GetComponent<TrajectoryPlanner>().Target = target;
    }
}
}
