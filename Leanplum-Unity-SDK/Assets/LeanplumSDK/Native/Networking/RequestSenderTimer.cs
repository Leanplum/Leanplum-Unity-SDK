using System;
using UnityEngine;

namespace LeanplumSDK
{
    // TODO: plug into start/resume
    public class RequestSenderTimer
    {
        private EventsUploadInterval timerInterval = EventsUploadInterval.AtMost15Minutes;

        public RequestSenderTimer()
        {
            Start();
        }

        public void Stop()
        {
            LeanplumUnityHelper.Instance.CancelInvoke(nameof(SendRequestsHeartbeat));
        }

        public void Start()
        {
            LeanplumUnityHelper.Instance.InvokeRepeating(nameof(SendRequestsHeartbeat), ((int)timerInterval) * 60, ((int)timerInterval) * 60);
        }

        private void SendRequestsHeartbeat()
        {
            Request request = RequestBuilder.withHeartbeatAction().CreateImmediate();
            Leanplum.RequestSender.Send(request);
        }
    }
}