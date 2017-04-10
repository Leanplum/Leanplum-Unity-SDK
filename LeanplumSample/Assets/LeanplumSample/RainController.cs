using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK;

public class RainController : MonoBehaviour {
    private Var<int> rainEmissionRate;
    private ParticleSystem rainSystem;
    private const int maxRate = 50000;

    void Awake() {
        // First, you need to setup your AndroidManifest.xml file to use push notifications.
        // See the documentation in Help > Docs > Unity > In-App & Push.
        Leanplum.SetGcmSenderId(Leanplum.LeanplumGcmSenderId);

        // Enables push notifications on iOS.
        Leanplum.RegisterForIOSRemoteNotifications();

        // Automatically tracks in-app purchases on iOS.
        // To track in-app purchases on Android,
        // use Leanplum.trackGooglePlayPurchase in your Android code when a purchase occurs.
        Leanplum.TrackIOSInAppPurchases();
    }

    void Start () {
        Application.runInBackground = true;

        rainSystem = GetComponent<ParticleSystem>();
        rainEmissionRate = Var.Define("rainEmissionRate", 2000);

        var rate = rainSystem.emission.rateOverTime;
        rainEmissionRate.ValueChanged += delegate() {
            rate.constantMax = rainEmissionRate.Value < maxRate ? rainEmissionRate.Value : maxRate;
        };
        rate.constantMax = rainEmissionRate.Value < maxRate ? rainEmissionRate.Value : maxRate;
    }
}
