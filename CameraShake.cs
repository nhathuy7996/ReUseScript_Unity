using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	Transform camTransform;
	
	// How long the object should shake for.
	float _shakeDuration = 0f;
    public float shakeDuration => _shakeDuration;
    [SerializeField]
    List<Color> BG_Color = new List<Color>();
	
    [SerializeField]
	// Amplitude of the shake. A larger value shakes the camera harder.
	float shakeAmount = 0.7f, decreaseFactor = 1.0f;

    public UnityEvent ShakeDone = null;
	
	Vector3 originalPos;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{
		if (_shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;	
			_shakeDuration -= Time.deltaTime * decreaseFactor;
            Camera.main.backgroundColor = BG_Color[1];
		}
		else
		{
			_shakeDuration = 0f;
			camTransform.localPosition = originalPos;
            Camera.main.backgroundColor = BG_Color[0];
		}
	}

    public void Shake(float timeDur){
        _shakeDuration = timeDur;
        StartCoroutine(CallEventDone());
    }

    IEnumerator CallEventDone(){
        yield return new WaitForSeconds(_shakeDuration);
        if(ShakeDone != null){
            ShakeDone.Invoke();
            ShakeDone.RemoveAllListeners();
        }

    }
}