using System;

namespace MvcMovie.Models
{
    public class AppRole
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
}
