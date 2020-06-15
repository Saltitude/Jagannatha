using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_UI_DownloadBar : MonoBehaviour
{
    Slider progressBar;
    bool updating = false;
    // Start is called before the first frame update
    void Start()
    {
        progressBar = this.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        progressBar.value = SC_SyncVar_DisplaySystem.Instance.Progress;
        updating = SC_SyncVar_DisplaySystem.Instance.Updating;
    }
}
