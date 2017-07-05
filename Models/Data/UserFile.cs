using System;

namespace SSUMAP.Models.Data
{
    public class UserFile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string OriginalName { get; set; }
        public string Mime { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }

        public bool IsPrivate { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}