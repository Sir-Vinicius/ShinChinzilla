
using MoreMountains.Feedbacks;
using UnityEngine;

public class BlockPointsSpawner : MonoBehaviour
{
    public static BlockPointsSpawner Instance { get; private set; }

    [SerializeField] private MMF_Player pointsFeedback;

    private MMF_FloatingText floatingText;

    private void Awake()
    {
        Instance = this;

        if (pointsFeedback != null)
        {
            floatingText =
                pointsFeedback.GetFeedbackOfType<MMF_FloatingText>();
        }
    }

    public void ShowPoints(Vector3 position, int points)
    {
        if (pointsFeedback == null || floatingText == null)
        {
            Debug.LogWarning("MMF Floating Text não configurado no BlockPointsSpawner!");
            return;
        }

        floatingText.Value = $"+{points}";

        pointsFeedback.PlayFeedbacks(position);
    }
}
