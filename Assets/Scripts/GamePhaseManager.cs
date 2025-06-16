using UnityEngine;

public class GamePhaseManager : MonoBehaviour
{
    [Header("Phase Objects")]
    [SerializeField] private GameObject canopyPhase;   // the whole canopy rig

    [Header("Camera AudioListeners")]
    [SerializeField] private AudioListener forestCamAudio;   // tree-phase camera
    [SerializeField] private AudioListener canopyCamAudio;   // canopy camera

    public FirstPersonCanopyController canopyController;   // assign prefab in inspector
    public Camera forestCamera;                            // existing follow-camera
    private void Start()
    {
        canopyPhase.SetActive(false);
        canopyCamAudio.enabled = false;      // make sure only one starts enabled
    }

    public void EnterCanopyPhase()
    {
        // disable forest cam & input
        forestCamera.gameObject.SetActive(false);

        // enable FP controller
        canopyController.gameObject.SetActive(true);

        canopyPhase.SetActive(true);
        SwitchToCanopyCamera();

        EnergyBank.instance?.BeginCanopyPhase();
        gameObject.SetActive(false);
    }

    private void SwitchToCanopyCamera()
    {
        forestCamAudio.enabled = false;
        canopyCamAudio.enabled = true;
    }
}
