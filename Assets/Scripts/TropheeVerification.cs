using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

public class TropheeVerification : MonoBehaviour
{
    private static bool isVerified = false;

    private VisualElement root;
    private TextField clientIDField;
    private TextField gameIDField;
    private Button verifyButton;
    private Label statusLabel;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        clientIDField = root.Q<TextField>("ClientID");
        gameIDField = root.Q<TextField>("GameID");
        verifyButton = root.Q<Button>("VerifyButton");
        statusLabel = root.Q<Label>("StatusLabel");

        verifyButton.clicked += VerifyClient;
    }

    private void VerifyClient()
    {
        string clientID = clientIDField.value;
        string gameID = gameIDField.value;
        statusLabel.text = "Verifying...";
        StartCoroutine(SendVerificationRequest(clientID, gameID));
    }

    private IEnumerator SendVerificationRequest(string clientID, string gameID)
    {
        string url = "https://trophee.live/partner/api/v1/verify-client";
        string jsonData = "{\"client_id\":\"" + clientID + "\", \"game_id\":\"" + gameID + "\"}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            statusLabel.text = "Verification Successful!";
            isVerified = true;
            UpdateMenuCheckmark();
        }
        else
        {
            statusLabel.text = "Incorrect credentials, please try again.";
        }
    }

    private static void UpdateMenuCheckmark()
    {
        Menu.SetChecked("Trophee/Verify", isVerified);
    }

    [MenuItem("Trophee/Verify", false, 0)]
    public static void TropheeMenu()
    {
        if (!isVerified)
        {
            EditorUtility.DisplayDialog("Verification Required", "Please verify your credentials to access this menu.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Verification Successful", "You have successfully verified! The Trophee menu is now active.", "OK");
        }
    }
}