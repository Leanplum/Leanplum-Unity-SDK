using System.Collections;
using UnityEngine;

namespace LeanplumSDK
{
    public class InAppPanel : MonoBehaviour
    {
        protected InAppModel Model { get; set; }

        internal static void Create(InAppModel model)
        {
            InAppPrefabPlaceholder panel = FindObjectOfType<InAppPrefabPlaceholder>();

            GameObject gameObject = new GameObject($"InAppGameObject:{model.Context}");
            gameObject.AddComponent<InAppPanel>();
            gameObject.AddComponent<CanvasGroup>();

            var iamPanel = gameObject.GetComponent<InAppPanel>();
            iamPanel.Model = model;

            gameObject.transform.SetParent(panel.transform.parent);
        }

        // Use this for initialization
        void Start()
        {
            InAppPrefabPlaceholder panelPlaceholder = FindObjectOfType<InAppPrefabPlaceholder>();
            GameObject prefab = panelPlaceholder.inAppPrefab;
            GameObject inApp = Instantiate(prefab, gameObject.transform);
            inApp.name = $"InApp:{Model.Context}";

            var message = inApp.GetComponentInChildren<InAppPrefab>();
            message.Title.text = Model.Title;
            message.MessageText.text = Model.Message;

            message.AcceptButton.onClick.AddListener(() =>
            {
                Model.OnAccept?.Invoke();
                StartCoroutine(FadeOut());
            });

            if (!Model.HasCancelButton)
            {
                message.CancelButton.gameObject.SetActive(false);
            }
            else
            {
                message.CancelButton.onClick.AddListener(() =>
                {
                    Model.OnCancel?.Invoke();
                    StartCoroutine(FadeOut());
                });
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator FadeOut()
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.deltaTime * 2;
                yield return null;
            }
            Destroy(gameObject);
            Model.Context.Dismissed();
            yield return null;
        }
    }
}