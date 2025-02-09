public static class GameEnums
{
    public enum StoryPathType
    {
        // Simple sequence (Level 1 → Level 2)
        Linear,
        // Max 2 next levels per level
        Branching,
        // Procedurally generated levels
        Random
    }

    public enum Difficulty
    {
        Easy, Normal, Hard
    }
}
