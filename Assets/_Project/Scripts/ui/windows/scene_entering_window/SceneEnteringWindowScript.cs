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
		if (GameOverlayWindowScript.instance != null) {
			GameOverlayWindowScript.instance.HideOverlay();
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

	public void CheckTutorial()
	{
		if (SceneManager.instance != null && DataBaseManager.instance != null)
		{
			int savedBuildingCount = DataBaseManager.instance.GetSavedBuildingCount();
			List<int> claimedMissions = DataBaseManager.instance.GetClaimedMissionIds();

			// Tutorial is active if no buildings exist, OR if only 1 building exists but its mission hasn't been claimed
			if (savedBuildingCount == 0)
			{
				SceneManager.instance.isTutorialActive = true;
				WindowScript window = UIManager.instance.ShowTutorialWindow();
				if (window != null && window is TutorialWindowScript tut)
				{
					tut.ShowWindow();
				}
			}
			else if (savedBuildingCount == 1 && (claimedMissions == null || claimedMissions.Count == 0))
			{
				SceneManager.instance.isTutorialActive = true;
				// If building is already finished, ProductionScript will trigger the TutorialWindow
				// If building is still under construction, UIManager will block the normal overlay
			}
			else
			{
				SceneManager.instance.isTutorialActive = false;
				if (GameOverlayWindowScript.instance != null)
				{
					GameOverlayWindowScript.instance.SetTutorialState(false);
				}
			}
		}
	}

	public override void Close()
	{
		this.CheckTutorial();
		base.Close();
	}
}
