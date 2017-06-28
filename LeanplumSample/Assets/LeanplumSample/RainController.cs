using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LeanplumSDK;

public class RainController : MonoBehaviour
{
    private Var<int> rainEmissionRate;
    private ParticleSystem rainSystem;
    private const int maxRate = 50000;

    void Awake()
    {
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

    void Start()
    {
        Application.runInBackground = true;
        rainSystem = GetComponent<ParticleSystem>();
        rainEmissionRate = Var.Define("rainEmissionRate", 2000);
        rainEmissionRate.ValueChanged += delegate ()
        {
            Debug.Log("Changed rainEmissionRate to " + rainEmissionRate.Value);
            UpdateEmissionRate(rainEmissionRate.Value < maxRate ? rainEmissionRate.Value : maxRate);
        };
        Debug.Log("Started with rainEmissionRate of " + rainEmissionRate.Value);
        UpdateEmissionRate(rainEmissionRate.Value < maxRate ? rainEmissionRate.Value : maxRate);
    }

    private void UpdateEmissionRate(float particlesPerSecond)
    {
        var em = rainSystem.emission;
        var newRate = new ParticleSystem.MinMaxCurve();
        newRate.constantMax = particlesPerSecond;
#if UNITY_5_6_OR_NEWER
        em.rateOverTime = newRate;
#else
        em.rate = newRate;
#endif
    }
}
