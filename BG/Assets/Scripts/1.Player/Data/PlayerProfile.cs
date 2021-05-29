using UnityEngine;

[CreateAssetMenu(fileName = "Player Profile", menuName = "Data/Player Profile")]
public class PlayerProfile : ScriptableObject {
    [SerializeField] float moveSpeed = 1F;
    [SerializeField] float mouseSpeedHorizontal = 1F;
    [SerializeField] float mouseSpeedVertical = 1F;
    [SerializeField, Range(0.5f, 20F)] float pointDistance = 10F;

    public float MoveSpeed => moveSpeed;
    public float MouseSpeedHorizontal => mouseSpeedHorizontal;
    public float MouseSpeedVertical => mouseSpeedVertical;
    public float PointDistance => pointDistance;
}