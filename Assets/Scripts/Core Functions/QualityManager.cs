using QFSW.QC;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class QualityManager : MonoBehaviour
{
    public Volume PostProcessing;
    public MotionBlur motionBlur;

    [Command]
    public void ChangeQualitySettings(int Index)
    {
        Index--;
        QualitySettings.SetQualityLevel(Index, true);
    }

    [Command]
    public void ToggleMotionBlur()
    {
        PostProcessing.profile.TryGet(out MotionBlur motionBlur);
        
        if(motionBlur.intensity.value == 2)
        {
            Debug.Log("Motion Blur Disabled");
            motionBlur.intensity.value = 0;
        }
        else
        {
            Debug.Log("Motion Blur Enabled");
            motionBlur.intensity.value = 2;
        }
        
    }
}
