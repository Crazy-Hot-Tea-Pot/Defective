public class Inspector : Enemy
{
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Inspector";

        base.Start();
    }
    protected override void PerformIntent()
    {
        base.PerformIntent();

        StartCoroutine(PrepareToEndTurn());
    }

}