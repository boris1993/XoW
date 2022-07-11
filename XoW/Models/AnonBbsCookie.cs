using System;

namespace XoW.Models
{
    public class AnonBbsCookie : IEquatable<AnonBbsCookie>
    {
        public string Cookie { get; set; }
        public string Name { get; set; }

        public bool Equals(AnonBbsCookie other) => other.Cookie == Cookie && other.Name == Name;
    }
}
