using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSceneView : MonoBehaviour {
    public GameObject wind;
	
	// Update is called once per frame
	void Update () {
        if (wind != null)
        {
            wind.SetActive(RelaxingRhythmsGameView.Instance.playing);
        }
        AfterUpdate();
	}

    public virtual void AfterUpdate() { }
}
