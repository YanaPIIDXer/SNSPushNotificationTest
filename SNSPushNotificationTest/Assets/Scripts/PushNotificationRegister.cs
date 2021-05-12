using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Amazon.SimpleNotificationService;
using Amazon.Runtime;
using Amazon.CognitoIdentity;
using Amazon;
using Amazon.SimpleNotificationService.Model;

/// <summary>
/// プッシュ通知登録
/// </summary>
public class PushNotificationRegister : MonoBehaviour
{
    /// <summary>
    /// リージョン
    /// </summary>
    private static readonly string Region = "ap-northeast-1";

    /// <summary>
    /// IDプールのID
    /// </summary>
    private static readonly string IdentityPoolId = "ap-northeast-1:07e20c63-b62e-4868-bcba-19400190ede2";

    /// <summary>
    /// SNSアプリケーションのARN
    /// </summary>
    private static readonly string ApplicationArn = "arn:aws:sns:ap-northeast-1:310815347645:app/APNS_SANDBOX/Test";

    /// <summary>
    /// トピックのARN
    /// </summary>
    private static readonly string TopicArn = "arn:aws:sns:ap-northeast-1:310815347645:Test";

#if UNITY_IOS
    void Start()
    {
        UnityInitializer.AttachToGameObject(gameObject);
        // ↓Unity2017以降のバージョンではこの設定が必要らしい
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        
        UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound);

        StartCoroutine(TryGetToken((Token) =>
        {
            Debug.Log("Device Token:" + Token);

            var Credential = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.GetBySystemName(Region));
            var SNSClient = new AmazonSimpleNotificationServiceClient(Credential, RegionEndpoint.GetBySystemName(Region));
            SNSClient.CreatePlatformEndpointAsync(
                new CreatePlatformEndpointRequest
                {
                    Token = Token,
                    PlatformApplicationArn = ApplicationArn
                },
                (Result) =>
                {
                    if (Result.Exception != null)
                    {
                        Debug.LogError("CreatePlatformEndpoint Failed.");
                        Debug.LogError(Result.Exception.Message);
                        return;
                    }
                    Debug.Log("CreatePlatformEndpoint Success!");
                    SNSClient.SubscribeAsync(
                        new SubscribeRequest
                        {
                            Endpoint = Result.Response.EndpointArn,
                            Protocol = "application",
                            TopicArn = TopicArn
                        },
                        (SubscribeResult) =>
                    {
                        if (SubscribeResult.Exception != null)
                        {
                            Debug.LogError("Subscribe Failed.");
                            Debug.LogError(SubscribeResult.Exception.Message);
                            return;
                        }
                        Debug.Log("Subscribe Success!");
                    });
                }
            );
        }, () => Debug.Log("TryGetToken Failed.")));
    }

    /// <summary>
    /// トークン取得
    /// </summary>
    /// <param name="OnSuccess">成功コールバック</param>
    /// <param name="OnFail">失敗コールバック</param>
    private IEnumerator TryGetToken(Action<string> OnSuccess, Action OnFail)
    {
        for (int TryCount = 10; TryCount > 0; TryCount--)
        {
            yield return new WaitForSeconds(1.0f);
            var Token = UnityEngine.iOS.NotificationServices.deviceToken;
            var Error = UnityEngine.iOS.NotificationServices.registrationError;
            if (!string.IsNullOrEmpty(Error))
            {
                Debug.Log("Error:" + Error);
                OnFail?.Invoke();
                yield break;
            }

            if (Token != null)
            {
                string TokenStr = BitConverter.ToString(Token).Replace("-","");
                OnSuccess?.Invoke(TokenStr);
                yield break;
            }
        }
        OnFail?.Invoke();
    }
#endif
}
