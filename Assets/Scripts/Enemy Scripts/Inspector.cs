public class Inspector : Enemy
{
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Inspector";

        EnemyType = EnemyManager.TypeOfEnemies.Inspector;

        base.Start();
    }
    protected override void PerformIntent()
    {
        base.PerformIntent();

        StartCoroutine(PrepareToEndTurn());
    }

}