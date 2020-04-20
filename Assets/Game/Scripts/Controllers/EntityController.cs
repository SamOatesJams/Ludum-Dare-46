using UnityEngine;
using SamOatesGames.Systems;

public class EntityController : SubscribableMonoBehaviour
{
    [Header("Health")]
    public double MaxHealth;
    public double Health { get; set; }
}
