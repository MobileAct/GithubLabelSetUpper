using System;
using System.Runtime.Serialization;
using Core;
using YamlDotNet.Serialization;

namespace GitlabLabelSetUpper
{
    public class Label : ILabel
    {
        private static class Key
        {
            public const string Id = "id";
            public const string Name = "name";
            public const string Color = "color";
            public const string Description = "description";
            public const string Priority = "priority";
            public const string Aliases = "aliases";
        }

        [DataMember(Name = Key.Id, IsRequired = false)]
        [YamlMember(Alias = Key.Id, ApplyNamingConventions = false)]
        public int? Id { get; set; }

        [DataMember(Name = Key.Name, IsRequired = true)]
        [YamlMember(Alias = Key.Name, ApplyNamingConventions = false)]
        public string? Name { get; set; }

        [DataMember(Name = Key.Color, IsRequired = true)]
        [YamlMember(Alias = Key.Color, ApplyNamingConventions = false)]
        public string? Color { get; set; }

        [DataMember(Name = Key.Description, IsRequired = false)]
        [YamlMember(Alias = Key.Description, ApplyNamingConventions = false)]
        public string? Description { get; set; }

        [DataMember(Name = Key.Priority, IsRequired = false)]
        [YamlMember(Alias = Key.Priority, ApplyNamingConventions = false)]
        public int? Priority { get; set; }

        [DataMember(Name = Key.Aliases, IsRequired = false)]
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
                return $"Id: {Id}, Name: {Name}, Color: {Color}, Description: {Description}, Priority: {Priority}";
            }
            else
            {
                return $"Id: {Id}, Name: {Name}, Color: {Color}, Description: {Description}, Priority: {Priority}, Aliases: {string.Join(',', Aliases)}";
            }
        }
    }
}
