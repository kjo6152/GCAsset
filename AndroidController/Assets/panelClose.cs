using UnityEngine;
using System.Collections;

public class panelClose : MonoBehaviour {


    public void close() { 
        gameObject.transform.parent.gameObject.SetActive(false);
    }

}
