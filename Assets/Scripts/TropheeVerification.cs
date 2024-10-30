using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

public class TropheeVerification : MonoBehaviour
{
    private static bool isVerified = false; // Track verification status

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
            AddMenuItem(); // Call method to add menu item
        }
        else
        {
            statusLabel.text = "Incorrect credentials, please try again.";
        }
    }

    // Adds the menu item dynamically
    private static void AddMenuItem()
    {
        EditorApplication.delayCall += () =>
        {
            Menu.SetChecked("Trophee/Verify", true);  // Set the submenu item as active
        };
    }

    [MenuItem("Trophee/Verify", false, 0)]
    public static void TropheeMenu()
    {
        if (!isVerified)
        {
            Debug.Log("Verification required to access this menu.");
        }
        else
        {
            Debug.Log("Trophee menu is active!");
            // Additional functionality after verification
        }
    }
}
