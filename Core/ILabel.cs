namespace Core
{
    public interface ILabel
    {
        public string? Name { get; }

        public string? Color { get; }

        public string? Description { get; set; }

        public string[]? Aliases { get; set; }
    }
}
