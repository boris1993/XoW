using System;

namespace XoW.Models
{
    public class AnoBbsCookie : IEquatable<AnoBbsCookie>
    {
        public string Cookie { get; set; }
        public string Name { get; set; }

        public bool Equals(AnoBbsCookie other) => other?.Cookie == Cookie && other?.Name == Name;
    }
}
