public class Enemy : Entity
{
    protected override void Start()
    {
        base.Start();

        ObjectsInWorld.Instance.AddEnemyToList(this);
    }

    private void Update()
    {
        if (nearestFoe != null)
        {
            if (!Locomotion.CloseEnough(nearestFoe.transform.position))
            {
                Locomotion.RotateAndMove(nearestFoe.transform.position);
                return;
            }
            else
            {
                Combat.Attack();
                Locomotion.SetMoveAnimation(false);
                return;
            }
        }
        
        Locomotion.SetMoveAnimation(false);
    }

    private void FixedUpdate()
    {
        nearestFoe = FindNearestFoe(ObjectsInWorld.Instance.AlliedObjects, false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ObjectsInWorld.Instance.RemoveEnemyFromList(this);
    }
}
