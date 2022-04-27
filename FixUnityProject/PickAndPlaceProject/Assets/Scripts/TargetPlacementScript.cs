using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.Robotics.PickAndPlace
{

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    public class TargetPlacementScript : MonoBehaviour
    {
        public GameObject GrabbableObjects;
        public GameObject Displays;
        public GameObject Blockbuttons;
        public GameObject OpenLinkButton;
        public GameObject ExitDisplayButton;
        public GameObject ToggleButton;
        // BUTTONS
        public Button WieBenIkButton;
        public Button DigitaleVoorstellingButton;
        public Button ExcellentieprogrammaButton;
        public Button BrainjarButton;
        public Button ExtraActiviteitenButton;
        public Button GitHubProjectenButton;
        public Button ComputerwetenschappenButton;
        public Button XFactorButton;

        // PREFABS
        public GameObject WieBenIkPrefab;
        public GameObject DigitaleVoorstellingPrefab;
        public GameObject ExcellentieprogrammaPrefab;
        public GameObject BrainjarPrefab;
        public GameObject ExtraActiviteitenPrefab;
        public GameObject GitHubProjectenPrefab;
        public GameObject ComputerwetenschappenPrefab;
        public GameObject XFactorPrefab;

        // TRANSFORMS
        private Vector3 WieBenIkObjectPosition;
        private Quaternion WieBenIkObjectRotation;
        private Vector3 DigitaleVoorstellingObjectPosition;
        private Quaternion DigitaleVoorstellingObjectRotation;
        private Vector3 ExcellentieprogrammaObjectPosition;
        private Quaternion ExcellentieprogrammaObjectRotation;
        private Vector3 BrainjarPosition;
        private Quaternion BrainjarRotation;
        private Vector3 ExtraActiviteitenPosition;
        private Quaternion ExtraActiviteitenRotation;
        private Vector3 GitHubProjectenPosition;
        private Quaternion GitHubProjectenRotation;
        private Vector3 ComputerwetenschappenPosition;
        private Quaternion ComputerwetenschappenRotation;
        private Vector3 XFactorPosition;
        private Quaternion XFactorRotation;

        // DISPLAYS
        public GameObject WieBenIkDisplay;
        public GameObject CVDisplay;
        public GameObject DigitaleVoorstellingDisplay;
        public GameObject ExcellentieprogrammaDisplay;
        public GameObject VideoDisplay;
        public GameObject BrainjarDisplay;
        public GameObject ExtraActiviteitenDisplay;
        public GameObject GitHubProjectenDisplay;
        public GameObject sixnimmtDisplay;
        public GameObject sixnimmtMirrorDisplay;
        public GameObject SADRIFUNDisplay;
        public GameObject ComputerwetenschappenDisplay;
        public GameObject XFactorDisplay;



        public const string k_NameExpectedTarget = "Target";
        static readonly int k_ShaderColorId = Shader.PropertyToID("_Color");
        // The threshold that the Target's speed must be under to be considered "placed" in the target area
        const float k_MaximumSpeedForStopped = 0.01f;

        [SerializeField]
        [Tooltip("Target object expected by this placement area. Can be left blank if only one Target in scene")]
        public GameObject m_Target;
        [SerializeField]
        [Range(0, 255)]
        [Tooltip("Alpha value for any color set during state changes.")]
        int m_ColorAlpha = 100;

        MeshRenderer m_TargetMeshRenderer;

        float m_ColorAlpha01 => m_ColorAlpha / 255f;
        MeshRenderer m_MeshRenderer;
        BoxCollider m_BoxCollider;
        PlacementState m_CurrentState;
        PlacementState m_LastColoredState;

        public PlacementState CurrentState
        {
            get => m_CurrentState;
            private set
            {
                m_CurrentState = value;
                // UpdateStateColor();
            }
        }

        public enum PlacementState
        {
            Outside,
            InsideFloating,
            InsidePlaced
        }

        // Start is called before the first frame update
        void Start()
        {
            // GET OBJECT TRANSFORMS
            WieBenIkObjectPosition = GameObject.Find("WieBenIk").gameObject.transform.position;
            WieBenIkObjectRotation = GameObject.Find("WieBenIk").gameObject.transform.rotation;

            DigitaleVoorstellingObjectPosition = GameObject.Find("DigitaleVoorstelling").gameObject.transform.position;
            DigitaleVoorstellingObjectRotation = GameObject.Find("DigitaleVoorstelling").gameObject.transform.rotation;

            ExcellentieprogrammaObjectPosition = GameObject.Find("Excellentieprogramma").gameObject.transform.position;
            ExcellentieprogrammaObjectRotation = GameObject.Find("Excellentieprogramma").gameObject.transform.rotation;

            BrainjarPosition = GameObject.Find("Brainjar").gameObject.transform.position;
            BrainjarRotation = GameObject.Find("Brainjar").gameObject.transform.rotation;

            ExtraActiviteitenPosition = GameObject.Find("ExtraActiviteiten").gameObject.transform.position;
            ExtraActiviteitenRotation = GameObject.Find("ExtraActiviteiten").gameObject.transform.rotation;

            GitHubProjectenPosition = GameObject.Find("GitHubProjecten").gameObject.transform.position;
            GitHubProjectenRotation = GameObject.Find("GitHubProjecten").gameObject.transform.rotation;

            ComputerwetenschappenPosition = GameObject.Find("Computerwetenschappen").gameObject.transform.position;
            ComputerwetenschappenRotation = GameObject.Find("Computerwetenschappen").gameObject.transform.rotation;

            XFactorPosition = GameObject.Find("X-Factor").gameObject.transform.position;
            XFactorRotation = GameObject.Find("X-Factor").gameObject.transform.rotation;

            // Check for mis-configurations and disable if something has changed without this script being updated
            // These are warnings because this script does not contain critical functionality
            if (m_Target == null)
            {
                m_Target = GameObject.Find(k_NameExpectedTarget);
            }

            if (m_Target == null)
            {
                Debug.LogWarning($"{nameof(TargetPlacementScript)} expects to find a GameObject named " +
                    $"{k_NameExpectedTarget} to track, but did not. Can't track placement state.");
                enabled = false;
                return;
            }

            if (!TrySetComponentReferences())
            {
                enabled = false;
                return;
            }
            InitializeState();
        }

        bool TrySetComponentReferences()
        {
            m_TargetMeshRenderer = m_Target.GetComponent<MeshRenderer>();
            if (m_TargetMeshRenderer == null)
            {
                Debug.LogWarning($"{nameof(TargetPlacementScript)} expects a {nameof(MeshRenderer)} to be attached " +
                    $"to {k_NameExpectedTarget}. Cannot check bounds without it, so cannot track placement state.");
                return false;
            }

            // Assume these are here because they are RequiredComponent components
            m_MeshRenderer = GetComponent<MeshRenderer>();
            m_BoxCollider = GetComponent<BoxCollider>();
            return true;
        }

        void OnValidate()
        {
            // Useful for visualizing state in editor, but doesn't wholly guarantee accurate coloring in EditMode
            // Enter PlayMode to see color update correctly
            if (m_Target != null)
            {
                if (TrySetComponentReferences())
                {
                    InitializeState();
                }
            }
        }

        void InitializeState()
        {
            if (m_Target.GetComponent<BoxCollider>().bounds.Intersects(m_BoxCollider.bounds))
            {
                CurrentState = IsTargetStoppedInsideBounds() ?
                    PlacementState.InsidePlaced : PlacementState.InsideFloating;
            }
            else
            {
                CurrentState = PlacementState.Outside;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == m_Target.name)
            {
                if (other.gameObject.tag == "WieBenIk") {
                    // TRIGGER OPENEN VAN WieBenIk!!!
                    Debug.Log("WieBenIk");
                    other.name = "WieBenIk";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    // Application.OpenURL("https://www.linkedin.com/in/jasper-cremers-69b753200/");
                    Displays.SetActive(true);
                    WieBenIkDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);

                    OpenLinkButton.SetActive(true);
                    OpenLinkButton.GetComponent<Url>().UrlString = "https://www.linkedin.com/in/jasper-cremers-69b753200/";

                    ToggleButton.SetActive(true);
                    ToggleButton.GetComponent<ToggleDisplays>().ToggleBool = false;
                    ToggleButton.GetComponent<ToggleDisplays>().VideoBool = false;
                    ToggleButton.GetComponent<ToggleDisplays>().DisplayOne = WieBenIkDisplay;
                    ToggleButton.GetComponent<ToggleDisplays>().DisplayTwo = CVDisplay;

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(WieBenIkPrefab,WieBenIkObjectPosition,WieBenIkObjectRotation) as GameObject;
                    newObject.tag = "WieBenIk";
                    newObject.transform.parent = GrabbableObjects.transform;
                    WieBenIkButton.GetComponent<TargetSetup>().target = newObject;
                }

                if (other.gameObject.tag == "DigitaleVoorstelling") {
                    // TRIGGER OPENEN VAN DigitaleVoorstelling!!!
                    Debug.Log("DigitaleVoorstelling");
                    other.name = "DigitaleVoorstelling";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    Displays.SetActive(true);
                    DigitaleVoorstellingDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);
                    OpenLinkButton.SetActive(true);
                    OpenLinkButton.GetComponent<Url>().UrlString = "https://github.com/Unity-Technologies/Unity-Robotics-Hub";

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(DigitaleVoorstellingPrefab,DigitaleVoorstellingObjectPosition,DigitaleVoorstellingObjectRotation) as GameObject;
                    newObject.tag = "DigitaleVoorstelling";
                    newObject.transform.parent = GrabbableObjects.transform;
                    DigitaleVoorstellingButton.GetComponent<TargetSetup>().target = newObject;
                }

                if (other.gameObject.tag == "Excellentieprogramma") {
                    // TRIGGER OPENEN VAN VIDEO EXCELLENTIE PROGRAMMA!!!
                    Debug.Log("Excellentieprogramma");
                    other.name = "Excellentieprogramma";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    Displays.SetActive(true);
                    ExcellentieprogrammaDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);

                    ToggleButton.SetActive(true);
                    ToggleButton.GetComponent<ToggleDisplays>().ToggleBool = false;
                    ToggleButton.GetComponent<ToggleDisplays>().VideoBool = true;
                    ToggleButton.GetComponent<ToggleDisplays>().DisplayOne = ExcellentieprogrammaDisplay ;
                    ToggleButton.GetComponent<ToggleDisplays>().DisplayTwo = VideoDisplay;

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(ExcellentieprogrammaPrefab,ExcellentieprogrammaObjectPosition,ExcellentieprogrammaObjectRotation) as GameObject;
                    newObject.tag = "Excellentieprogramma";
                    newObject.transform.parent = GrabbableObjects.transform;
                    ExcellentieprogrammaButton.GetComponent<TargetSetup>().target = newObject;
                }

                if (other.gameObject.tag == "Brainjar") {
                    // TRIGGER OPENEN VAN Brainjar!!!
                    Debug.Log("Brainjar");
                    other.name = "Brainjar";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    Displays.SetActive(true);
                    BrainjarDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);
                    OpenLinkButton.SetActive(true);
                    OpenLinkButton.GetComponent<Url>().UrlString = "https://brainjar.ai/en/";

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(BrainjarPrefab,BrainjarPosition,BrainjarRotation) as GameObject;
                    newObject.tag = "Brainjar";
                    newObject.transform.parent = GrabbableObjects.transform;
                    BrainjarButton.GetComponent<TargetSetup>().target = newObject;
                }

                if (other.gameObject.tag == "ExtraActiviteiten") {
                    // TRIGGER OPENEN VAN ExtraActiviteiten!!!
                    Debug.Log("ExtraActiviteiten");
                    other.name = "ExtraActiviteiten";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    Displays.SetActive(true);
                    ExtraActiviteitenDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(ExtraActiviteitenPrefab,ExtraActiviteitenPosition,ExtraActiviteitenRotation) as GameObject;
                    newObject.tag = "ExtraActiviteiten";
                    newObject.transform.parent = GrabbableObjects.transform;
                    ExtraActiviteitenButton.GetComponent<TargetSetup>().target = newObject;
                }

                if (other.gameObject.tag == "GitHubProjecten") {
                    // TRIGGER OPENEN VAN GitHubProjecten!!!
                    Debug.Log("GitHubProjecten");
                    other.name = "GitHubProjecten";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    Displays.SetActive(true);
                    GitHubProjectenDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);

                    ToggleButton.SetActive(true);
                    ToggleButton.GetComponent<ToggleDisplays>().ToggleBool = true;
                    ToggleButton.GetComponent<ToggleDisplays>().VideoBool = false;
                    ToggleButton.GetComponent<ToggleDisplays>().ProjectDisplayOne = GitHubProjectenDisplay;
                    ToggleButton.GetComponent<ToggleDisplays>().ProjectDisplayTwo = sixnimmtDisplay;
                    ToggleButton.GetComponent<ToggleDisplays>().ProjectDisplayThree = sixnimmtMirrorDisplay;
                    ToggleButton.GetComponent<ToggleDisplays>().ProjectDisplayFour = SADRIFUNDisplay;

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(GitHubProjectenPrefab,GitHubProjectenPosition,GitHubProjectenRotation) as GameObject;
                    newObject.tag = "GitHubProjecten";
                    newObject.transform.parent = GrabbableObjects.transform;
                    GitHubProjectenButton.GetComponent<TargetSetup>().target = newObject;
                }

                if (other.gameObject.tag == "Computerwetenschappen") {
                    // TRIGGER OPENEN VAN Computerwetenschappen!!!
                    Debug.Log("Computerwetenschappen");
                    other.name = "Computerwetenschappen";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    Displays.SetActive(true);
                    ComputerwetenschappenDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);
                    OpenLinkButton.SetActive(true);
                    OpenLinkButton.GetComponent<Url>().UrlString = "https://www.vub.be/opleiding/ingenieurswetenschappen-computerwetenschappen#over-de-opleiding";

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(ComputerwetenschappenPrefab,ComputerwetenschappenPosition,ComputerwetenschappenRotation) as GameObject;
                    newObject.tag = "Computerwetenschappen";
                    newObject.transform.parent = GrabbableObjects.transform;
                    ComputerwetenschappenButton.GetComponent<TargetSetup>().target = newObject;
                }
                if (other.gameObject.tag == "X-Factor") {
                    // TRIGGER OPENEN VAN XFactor!!!
                    Debug.Log("X-Factor");
                    other.name = "X-Factor";
                    m_Target = null;
                    Destroy(other.gameObject,1);

                    Displays.SetActive(true);
                    XFactorDisplay.SetActive(true);
                    Blockbuttons.SetActive(false);
                    ExitDisplayButton.SetActive(true);

                    // INSTANTIATE NEW OBJECT
                    GameObject newObject;
                    newObject = Instantiate(XFactorPrefab,XFactorPosition,XFactorRotation) as GameObject;
                    newObject.tag = "X-Factor";
                    newObject.transform.parent = GrabbableObjects.transform;
                    XFactorButton.GetComponent<TargetSetup>().target = newObject;
                }

                CurrentState = PlacementState.InsideFloating;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == m_Target.name)
            {
                CurrentState = PlacementState.Outside;
            }
        }

        bool IsTargetStoppedInsideBounds()
        {
            var targetIsStopped = m_Target.GetComponent<Rigidbody>().velocity.magnitude < k_MaximumSpeedForStopped;
            var targetIsInBounds = m_BoxCollider.bounds.Contains(m_TargetMeshRenderer.bounds.center);

            return targetIsStopped && targetIsInBounds;
        }

        // Update is called once per frame
        void Update()
        {
            if (CurrentState != PlacementState.Outside)
            {
                CurrentState = IsTargetStoppedInsideBounds() ?
                    PlacementState.InsidePlaced : PlacementState.InsideFloating;
            }
        }

        // void UpdateStateColor()
        // {
        //     if (m_CurrentState == m_LastColoredState)
        //     {
        //         return;
        //     }

        //     var mpb = new MaterialPropertyBlock();
        //     Color stateColor;
        //     switch (m_CurrentState)
        //     {
        //         case PlacementState.Outside:
        //             stateColor = Color.red;
        //             break;
        //         case PlacementState.InsideFloating:
        //             stateColor = Color.yellow;
        //             break;
        //         case PlacementState.InsidePlaced:
        //             stateColor = Color.green;
        //             break;
        //         default:
        //             Debug.LogError($"No state handling implemented for {m_CurrentState}");
        //             stateColor = Color.magenta;
        //             break;
        //     }

        //     stateColor.a = m_ColorAlpha01;
        //     mpb.SetColor(k_ShaderColorId, stateColor);
        //     m_MeshRenderer.SetPropertyBlock(mpb);
        //     m_LastColoredState = m_CurrentState;
        // }
    }
}
