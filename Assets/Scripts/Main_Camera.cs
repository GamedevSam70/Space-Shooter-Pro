using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Camera : MonoBehaviour
{
    [SerializeField]
    private float _cameraShakeLength = 0.3f;
    private float _shakePosMax = 0.1f;
    
    public void CameraShake()
    {
        StartCoroutine(CameraShakeActive());
    }


    IEnumerator CameraShakeActive()
    {
        Vector3 _defaultCameraPos = this.transform.position;
        float _shakeTime = Time.time + _cameraShakeLength;

        while (Time.time < _shakeTime)
        {
            float randomX = Random.Range(-_shakePosMax, _shakePosMax);
            float randomY = Random.Range(-_shakePosMax, _shakePosMax);
            this.transform.position = new Vector3(randomX, randomY, transform.position.z);
            yield return new WaitForEndOfFrame();
        }

        this.transform.position = _defaultCameraPos;
        
    }
}
