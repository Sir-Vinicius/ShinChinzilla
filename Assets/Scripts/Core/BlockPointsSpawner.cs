
using MoreMountains.Feedbacks;
using UnityEngine;

public class BlockPointsSpawner : MonoBehaviour
{
    public static BlockPointsSpawner Instance { get; private set; }

    [SerializeField] private MMF_Player pointsFeedback;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPoints(Vector3 position)
    {
        if (pointsFeedback == null)
        {
            Debug.LogWarning("MMF_Player de pontos não configurado!");
            return;
        }

        pointsFeedback.PlayFeedbacks(position);
    }
}
