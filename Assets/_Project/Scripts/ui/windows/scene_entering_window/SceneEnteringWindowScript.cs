using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SceneEnteringWindowScript : WindowScript {

	public Action OnIntermediate;

	public void IntermediateAction(){
		if (OnIntermediate != null) {
			OnIntermediate.Invoke ();
		}
	}

	public void ZoomFrom15To10() {
		StartCoroutine(SmoothZoomRoutine(15f, 10f, 1.2f));
	}

	private IEnumerator SmoothZoomRoutine(float startSize, float endSize, float duration) {
		float t = 0;
		if (CameraManager.instance != null) {
			CameraManager.instance.MainCamera.orthographicSize = startSize;
			CameraManager.instance.ClampCamera();

			while (t < 1f) {
				t += Time.deltaTime / duration;
				float smoothT = Mathf.SmoothStep(0f, 1f, t);
				CameraManager.instance.MainCamera.orthographicSize = Mathf.Lerp(startSize, endSize, smoothT);
				CameraManager.instance.ClampCamera();
				yield return null;
			}

			CameraManager.instance.MainCamera.orthographicSize = endSize;
			CameraManager.instance.ClampCamera();
		}
	}

	void LateUpdate() {
		transform.SetAsLastSibling();
	}
}
