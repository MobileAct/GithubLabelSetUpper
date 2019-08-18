using System;
using System.Runtime.Serialization;
using Core;
using YamlDotNet.Serialization;

namespace GithubLabelSetUpper
{
    public class Label : ILabel
    {
        private static class Key
        {
            public const string Name = "name";
            public const string Color = "color";
            public const string Description = "description";
            public const string Aliases = "aliases";
        }

        [DataMember(Name = Key.Name, IsRequired = true)]
        [YamlMember(Alias = Key.Name, ApplyNamingConventions = false)]
        public string? Name { get; set; }

        [DataMember(Name = Key.Color, IsRequired = true)]
        [YamlMember(Alias = Key.Color, ApplyNamingConventions = false)]
        public string? Color { get; set; }

        [DataMember(Name = Key.Description)]
        [YamlMember(Alias = Key.Description, ApplyNamingConventions = false)]
        public string? Description { get; set; }

        [DataMember(Name = Key.Aliases)]
        [YamlMember(Alias = Key.Aliases, ApplyNamingConventions = false)]
        public string[]? Aliases { get; set; }

        public void ValidateOrThrow()
        {
            if (Name is null)
            {
                throw new ArgumentNullException(nameof(Name));
            }
            if (Color is null)
            {
                throw new ArgumentNullException(nameof(Color));
            }
        }

        public override string ToString()
        {
            if (Aliases is null)
            {
                return $"Name: {Name}, Color: {Color}, Description: {Description}";
            }
            else
            {
                return $"Name: {Name}, Color: {Color}, Description: {Description}, Aliases: {string.Join(',', Aliases)}";
            }
        }
    }
}
