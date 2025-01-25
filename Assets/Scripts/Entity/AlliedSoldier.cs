using System;
using Buildings;
using Environment;
using SaveLoadSystem;
using Unity.Netcode;
using UnityEngine;
using static AlliedSoldier;
using Random = UnityEngine.Random;

public class AlliedSoldier : Entity, IBind<UnitDataStruct>
{
    public Transform SelectedBorder { get; private set; }
    private Vector2 _selectedBorderPosition;
    [SerializeField] private int id = -1;

    private UnitData _unitData = new UnitData();
    
    private Vector2 _borderOffset;

    public struct UnitDataStruct : INetworkSerializable, ISaveable
    {
        public int id;
        public int currentHealth;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref id);
            serializer.SerializeValue(ref currentHealth);
        }
    }

    public void Bind(UnitDataStruct gameManagerData)
    {
        Health.SetCurrentHealthServerRPC(gameManagerData.currentHealth);
    }

    public void SaveData()
    {
        _unitData.id = id;
        _unitData.currentHealth = Health.GetCurrentHealth();
    }

    public UnitData GetUnitData() => _unitData;
    public void SetUnitsId(int id) => this.id = id;

    private void Start()
    {
        ObjectsInWorld.Instance.AddSoldierToList(this);
        GreenZoneBorders.Instance.OnBordersPositionChangedEvent += SelectBordersPosition;
        GreenZoneBorders.Instance.SelectBorder(this);
    }

    private void Update()
    {
        if (!IsHost) return;

        if (nearestFoe != null)
        {
            if (!Locomotion.CloseEnough(nearestFoe.transform.position, nearestFoe.GetComponent<BoxCollider2D>().size.x/2))
            {
                Locomotion.RotateAndMoveWithVelocity(nearestFoe.transform.position);
                return;
            }
            else
            {
                Combat.Attack();
                Locomotion.SetMoveAnimation(false);
                return;
            }
        }

        if (!IsOnBorder(_selectedBorderPosition))
        {
            Locomotion.RotateAndMoveWithVelocity(_selectedBorderPosition);
            return;
        }
        
        Locomotion.SetMoveAnimation(false);
    }

    private void FixedUpdate()
    {
        if (!IsHost) return;

        if (!GreenZoneBorders.Instance.IsBeyondGreenZoneBorders(transform.position) || GreenZoneBorders.Instance.IsBeyondDefaultGreenZoneBorders(transform.position))
        {
            nearestFoe = FindNearestFoe(ObjectsInWorld.Instance.Enemies, true);
            
            if (nearestFoe != null)
                Locomotion.Rotate(nearestFoe.transform.position);
        }
    }

    public void SelectBorder(Transform newBorder, Vector2 borderOffset)
    {
        SelectedBorder = newBorder;
        _borderOffset = borderOffset;
        SelectBordersPosition();
    }

    private void SelectBordersPosition()
    {
        _selectedBorderPosition = AddRandomToTargetPosition(SelectedBorder.position, 0f, 1.5f);
        _selectedBorderPosition.x += _borderOffset.x;
    }

    private bool IsOnBorder(Vector2 targetPosition)
    {
        return Math.Abs(transform.position.x - targetPosition.x) <= 0.1;
    }

    private static Vector2 AddRandomToTargetPosition(Vector2 oldPosition, float minRandomValue, float maxRandomValue)
    {
        Vector2 newPosition = new Vector2(oldPosition.x, oldPosition.y);

        minRandomValue = Math.Abs(minRandomValue);
        maxRandomValue = Math.Abs(maxRandomValue);

        if (newPosition.x < 0)
        {
            newPosition += new Vector2(Random.Range(minRandomValue, maxRandomValue), 0);
        }
        else
        {
            newPosition += new Vector2(Random.Range(-minRandomValue, -maxRandomValue), 0);
        }

        return newPosition;
    }
    
    public override void OnDestroy()
    {
        base.OnDestroy();
        ObjectsInWorld.Instance.RemoveSoldierFromList(this);
        GreenZoneBorders.Instance.RemoveFromBorder(this);
    }

    [Serializable]
    public class UnitData : ISaveable
    {
        public int id;
        public int currentHealth;
    }
}