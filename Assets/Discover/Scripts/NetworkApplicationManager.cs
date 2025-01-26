// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using System.Collections.Generic;
using Discover.Configs;
using Discover.Icons;
using Discover.Utilities;
using Fusion;
using UnityEngine;
using Discover;

namespace Discover
{
    public class NetworkApplicationManager : NetworkSingleton<NetworkApplicationManager>
    {
        public static Action OnInstanceCreated;

        [SerializeField] private AppList m_appList;

        public NetworkApplicationContainer CurrentApplication { get; set; }

        public Action<AppManifest> OnAppStarted;
        public Action OnAppClosed;

        protected override void InternalAwake()
        {
            OnInstanceCreated?.Invoke();
        }

        public string GetCurrentAppDisplayName()
        {
            if (CurrentApplication == null)
            {
                return string.Empty;
            }

            var manifest = m_appList.GetManifestFromName(CurrentApplication.AppName);
            return manifest.DisplayName;
        }

        public void LaunchApplication(string appName, Transform appAnchor)
        {
            var manifest = m_appList.GetManifestFromName(appName);
            LaunchApplication(manifest, appAnchor);
        }

        public void LaunchApplication(AppManifest appManifest, Transform appAnchor)
        {
            if (HasStateAuthority)
            {
                LaunchApplication(appManifest, appAnchor.position, appAnchor.rotation);
            }
            else
            {
                LaunchApplicationOnServerRPC(appManifest.UniqueName, appAnchor.position, appAnchor.rotation);
            }
        }

        public void CloseApplication()
        {
            Debug.Log("The current net app container is: " + CurrentApplication.ToString());
            if (CurrentApplication != null)
            {
                if (HasStateAuthority)
                {
                    StopApplication();
                }
                else
                {
                    StopApplicationOnServerRPC();
                }
                IconsManager.Instance.DeregisterIcon(CurrentApplication.AppName);
                var iconTransform = AppsManager.Instance.m_movingIcon.transform;
                if (iconTransform.TryGetComponent<OVRSpatialAnchor>(out var anchor))
                {
                    AppsManager.Instance.m_anchorManager.EraseAnchor(anchor, false,
                        (erasedAnchor, success) =>
                        {
                            if (success)
                            {
                                DestroyImmediate(anchor);
                                /*iconTransform.position = position;
                                iconTransform.rotation = rotation;
                                var newAnchor = m_movingIcon.gameObject.AddComponent<OVRSpatialAnchor>();
                                m_anchorManager.SaveAnchor(newAnchor, new SpatialAnchorSaveData()
                                {
                                    Name = appManifest.UniqueName
                                });*/
                            }
                            else
                            {
                                Debug.LogError("Failed to erase anchor");
                            }
                            AppsManager.Instance.m_movingIcon = null;
                        });
                }
            }
        }

        public void OnApplicationStart(NetworkApplicationContainer applicationContainer)
        {
            Debug.Log(applicationContainer);
            if (CurrentApplication != null)
            {
                Debug.LogError("There is already an application running");
            }

            CurrentApplication = applicationContainer;


            //FIX THIS
            IconsManager.Instance.DisableIcon(applicationContainer.AppName);
            //IconsManager.Instance.DisableIcons();

            OnAppStarted?.Invoke(m_appList.GetManifestFromName(applicationContainer.AppName));
        }

        public void OnApplicationClosed(NetworkApplicationContainer applicationContainer)
        {
            if (CurrentApplication != applicationContainer)
            {
                Debug.LogError("Trying to close a different application than the current one");
            }

            CurrentApplication = null;

            OnAppClosed?.Invoke();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void LaunchApplicationOnServerRPC(string appName, Vector3 position, Quaternion rotation)
        {
            var manifest = m_appList.GetManifestFromName(appName);
            LaunchApplication(manifest, position, rotation);
        }

        private void LaunchApplication(AppManifest appManifest, Vector3 position, Quaternion rotation)
        {
            /*if (CurrentApplication != null)
            {
                Debug.LogError($"An Application ({CurrentApplication.AppName}) is already running! " +
                               $"Not starting ({appManifest.DisplayName}) a new one!");
                return;
            }*/
            _ = Runner.Spawn(appManifest.AppPrefab, position, rotation,
                onBeforeSpawned: (_, obj) =>
                {
                    var app = obj.GetComponent<NetworkApplicationContainer>();
                    app.AppName = appManifest.UniqueName;
                });
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void StopApplicationOnServerRPC()
        {
            StopApplication();
        }

        private void StopApplication()
        {
            Debug.Log("Stopping Application");
            if (CurrentApplication != null)
            {
                CurrentApplication.Shutdown();
            }
        }

        [ContextMenu("Launch App 0")]
        private void TestLaunchApp0() => LaunchApplication(m_appList.AppManifests[0], transform);

        [ContextMenu("Launch App 1")]
        private void TestLaunchApp1() => LaunchApplication(m_appList.AppManifests[1], transform);

        [ContextMenu("Launch App 2")]
        private void TestLaunchApp2() => LaunchApplication(m_appList.AppManifests[2], transform);

        [ContextMenu("Close App")]
        private void TestCloseApp() => CloseApplication();
    }
}