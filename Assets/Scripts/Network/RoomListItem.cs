using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Button joinButton;

    private SessionInfo sessionInfo;
    private System.Action<SessionInfo> joinAction;

    public void Setup(SessionInfo info, System.Action<SessionInfo> onJoin)
    {
        sessionInfo = info;
        joinAction = onJoin;

        roomNameText.text = info.Name;
        playerCountText.text = $"{info.PlayerCount}/8";

        joinButton.onClick.AddListener(OnJoinClicked);
    }

    private void OnJoinClicked()
    {
        joinAction?.Invoke(sessionInfo);
    }
}