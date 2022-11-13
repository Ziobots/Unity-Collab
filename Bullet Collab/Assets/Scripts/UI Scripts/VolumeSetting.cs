/*******************************************************************************
* Name : MainMenu.cs
* Section Description : Used to change the master game volume in options.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/13/22  0.10                 MH              
********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class VolumeSetting : MonoBehaviour
{   
    
    public AudioMixer audioMixer;
    
    public void SetVolume (float volume){
        audioMixer.SetFloat("volume",volume);
    } 

    
}
