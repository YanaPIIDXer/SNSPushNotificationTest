using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
