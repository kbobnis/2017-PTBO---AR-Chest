//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace GoogleARCore.HelloAR {
	using System.Collections.Generic;
	using GoogleARCore;
	using UnityEngine;
	using UnityEngine.Rendering;

	/// <summary>
	/// Controls the HelloAR example.
	/// </summary>
	public class ScanningARController : MonoBehaviour {
		/// <summary>
		/// The first-person camera being used to render the passthrough camera image (i.e. AR background).
		/// </summary>
		public Camera FirstPersonCamera;

		/// <summary>
		/// A prefab for tracking and visualizing detected planes.
		/// </summary>
		public GameObject TrackedPlanePrefab;

		/// <summary>
		/// A model to place when a raycast from a user touch hits a plane.
		/// </summary>
		public GameObject AndyAndroidPrefab;

		/// <summary>
		/// A list to hold all planes ARCore is tracking in the current frame. This object is used across
		/// the application to avoid per-frame allocations.
		/// </summary>
		private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

		/// <summary>
		/// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
		/// </summary>
		private bool m_IsQuitting = false;

		private bool isScanning = true;
		public bool anyPlaneFound = false;

		public event Action<Chest> OnFinishedScanning;

		private GameObject instantiatedAnchoredGO;

		private List<GameObject> allPlaneObjects = new List<GameObject>();
		public UI ui;

		private void OnEnable() {
			if (instantiatedAnchoredGO != null) {
				Destroy(instantiatedAnchoredGO);
				instantiatedAnchoredGO = null;
			}
			isScanning = true;
			anyPlaneFound = false;
			ui.Say("Look at the floor and move camera");
			InstantiatePlanes(TrackableQueryFilter.All);
		}

		/// <summary>
		/// The Unity Update() method.
		/// </summary>
		public void Update() {
			if (!isScanning) {
				return;
			}

			_QuitOnConnectionErrors();

			if (Application.platform == RuntimePlatform.WindowsEditor) {
				if (Input.GetMouseButtonDown(0)) {
					PlaceChest(FirstPersonCamera.transform.forward * 3f);
				}
			}
			else if (Application.platform == RuntimePlatform.Android) {
				// Check that motion tracking is tracking.
				if (Frame.TrackingState != TrackingState.Tracking) {
					const int lostTrackingSleepTimeout = 15;
					Screen.sleepTimeout = lostTrackingSleepTimeout;
					return;
				}

				Screen.sleepTimeout = SleepTimeout.NeverSleep;

				// Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
				InstantiatePlanes(TrackableQueryFilter.New);

				// Disable the snackbar UI when no planes are valid.
				Frame.GetPlanes(m_AllPlanes);
				for (int i = 0; i < m_AllPlanes.Count; i++) {
					if (m_AllPlanes[i].TrackingState == TrackingState.Tracking) {
						if (!anyPlaneFound) {
							anyPlaneFound = true;
							ui.Say("Click dot to place chest.");
						}
						break;
					}
				}
				//Editor hack
				Touch touch = Input.GetTouch(0);
				if ((Input.touchCount > 0 && touch.phase == TouchPhase.Began)) {
					// Raycast against the location the player touched to search for planes.
					TrackableHit hit;
					TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

					if (Session.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit)) {
						// Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
						// world evolves.
						var anchor = hit.Trackable.CreateAnchor(hit.Pose);
						GameObject andyObject = PlaceChest(hit.Pose.position);
						// Make Andy model a child of the anchor.
						andyObject.transform.parent = anchor.transform;
					}
				}
			}
		}

		private GameObject PlaceChest(Vector3 posePosition) {
			if (instantiatedAnchoredGO != null) {
				throw new Exception("Already initialized.");
			}
			// Andy should look at the camera but still be flush with the plane.
			GameObject andyObject = Instantiate(AndyAndroidPrefab, posePosition, Quaternion.identity);
			andyObject.transform.LookAt(FirstPersonCamera.transform);
			andyObject.transform.rotation = Quaternion.Euler(0.0f,
				andyObject.transform.rotation.eulerAngles.y, andyObject.transform.rotation.z);
			instantiatedAnchoredGO = andyObject;

			isScanning = false;
			foreach (GameObject planeObject in allPlaneObjects) {
				Destroy(planeObject);
			}
			allPlaneObjects.Clear();
			if (OnFinishedScanning != null) {
				OnFinishedScanning(andyObject.GetComponentInChildren<Chest>());
			}

			return andyObject;
		}

		private void InstantiatePlanes(TrackableQueryFilter filter) {
			List<TrackedPlane> planes = new List<TrackedPlane>();
			Frame.GetPlanes(planes, filter);

			for (int i = 0; i < planes.Count; i++) {
				// Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
				// the origin with an identity rotation since the mesh for our prefab is updated in Unity World
				// coordinates.
				GameObject planeObject = Instantiate(TrackedPlanePrefab, planes[i].Position, Quaternion.identity, transform);
				planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(planes[i]);
				allPlaneObjects.Add(planeObject);
			}
		}


		/// <summary>
		/// Quit the application if there was a connection error for the ARCore session.
		/// </summary>
		private void _QuitOnConnectionErrors() {
			if (m_IsQuitting) {
				return;
			}

			// Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
			if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission) {
				_ShowAndroidToastMessage("Camera permission is needed to run this application.");
				m_IsQuitting = true;
				Invoke("DoQuit", 0.5f);
			}
			else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed) {
				_ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
				m_IsQuitting = true;
				Invoke("DoQuit", 0.5f);
			}
		}

		/// <summary>
		/// Actually quit the application.
		/// </summary>
		private void DoQuit() {
			Application.Quit();
		}

		/// <summary>
		/// Show an Android toast message.
		/// </summary>
		/// <param name="message">Message string to show in the toast.</param>
		private void _ShowAndroidToastMessage(string message) {
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

			if (unityActivity != null) {
				AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
				unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
					AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
						message, 0);
					toastObject.Call("show");
				}));
			}
		}
	}
}